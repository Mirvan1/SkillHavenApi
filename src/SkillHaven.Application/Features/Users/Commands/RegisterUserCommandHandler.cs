using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using SkillHaven.Shared.User.Mail;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer _localizer;
        private readonly IMailService _mailService;
        public RegisterUserCommandHandler(IUserRepository userRepository, IConfiguration configuration, IMailService mailService)
        {
            _userRepository=userRepository;
            _configuration = configuration;
            _localizer=new Localizer();
            _mailService=mailService;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var checkEmail = _userRepository.GetByEmail(request.Email);//buraya get by email eklenecek;

            if (checkEmail is not null)
                throw new DatabaseValidationException(_localizer["Conflict", "Errors", "Email"].Value);

            if (!string.IsNullOrEmpty(request.Password))
                request.Password=BCrypt.Net.BCrypt.HashPassword(request.Password);

            //if (!IsPathValid(request.ProfilePicture))
            //    throw new DatabaseValidationException("Something wrong ... in photo");


            var newUser = new User()
            {
                Email=request.Email,
                FirstName=request.FirstName,
                LastName=request.LastName,
                Password=request.Password,
                ProfilePicture=SavePhoto(request.ProfilePicture, request.FirstName)??null,
                Role=request.Role.ToString()
            };

            if (request.Role == Role.Supervisor && request.SupervisorInfo!=null)
            {
                var newSupervisor = new Supervisor()
                {
                    Expertise=request.SupervisorInfo?.Expertise,
                    Description=request.SupervisorInfo?.Description
                };
                newUser.Supervisor=newSupervisor;
            }

            if (request.Role == Role.Consultant && request.ConsultantInfo!=null)
            {
                var newConsultant = new Consultant()
                {
                    Experience=request.ConsultantInfo.Experience,
                    Description=request.ConsultantInfo.Description
                };
                newUser.Consultant=newConsultant;
            }

            _userRepository.Add(newUser);
            int result = _userRepository.SaveChanges();

            //send email mehod impl

            await _mailService.SendEmail(
                new Shared.User.Mail.MailInfo()
                {
                    MailType=MailType.PlainText,
                    EmailBody=$"Welcome to our app-{newUser.FirstName}{newUser.LastName}",
                    EmailToName=newUser.FirstName+newUser.LastName,
                    EmailSubject="Registratation",
                    EmailToId=newUser.Email
                }
            );
            return await Task.FromResult(result>0);

        }


        static bool IsPathValid(string path)
        {
            return Path.IsPathRooted(path) && (Path.GetInvalidPathChars().All(c => path.IndexOf(c) == -1));
        }

        private string SavePhoto(string photoBase64, string name)
        {
            if (string.IsNullOrEmpty(photoBase64))
                return null;

            string imagesFolderPath = _configuration["ImagesFolderPath"];
            string extension = _configuration["ImageExtension"];

            if (!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }
            byte[] imageBytes = Convert.FromBase64String(photoBase64);

            string fileName = name+"_"+DateTime.Now.ToFileTime();

            string filePath = Path.Combine($"{imagesFolderPath}.{extension}", fileName);

            File.WriteAllBytes(filePath, imageBytes);

            return filePath;
        }
    }
}
