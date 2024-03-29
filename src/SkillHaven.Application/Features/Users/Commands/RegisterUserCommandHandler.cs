﻿using AutoMapper;
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
using SkillHaven.Shared.UtilDtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUtilService _utilService;
        private readonly IStringLocalizer _localizer;
        private readonly IMailService _mailService;
        public RegisterUserCommandHandler(IUserRepository userRepository, IUtilService utilService, IMailService mailService)
        {
            _userRepository=userRepository;
            _utilService = utilService;
            _localizer=new Localizer();
            _mailService=mailService;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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
                ProfilePicture=_utilService.SavePhoto(request.ProfilePicture, PhotoTypes.UserPhoto.ToString()+"_"+ request.FirstName)??null,
                Role=request.Role.ToString(),
                HasMailConfirm=false
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

            Random random = new Random();
            string verificationCode=random.Next(0, 999999).ToString();

            string htmlConfirmMessage = File.ReadAllText(Directory.GetCurrentDirectory()+"/StaticFiles/confirm-code-page.html");
            htmlConfirmMessage=htmlConfirmMessage.Replace("{ConfirmationCode}", verificationCode);


            var mailSendResult = await _mailService.SendEmail(
             new Shared.User.Mail.MailInfo()
             {
                 MailType=MailType.Html,
                 //EmailBody=$"Welcome to our app-{newUser.FirstName}{newUser.LastName}:Your Confirm Code:{verificationCode}",
                 EmailBody=htmlConfirmMessage,
                 EmailToName=newUser.FirstName+newUser.LastName,
                 EmailSubject="Registratation",
                 EmailToId=newUser.Email
             }
         );
            if (mailSendResult.Item1 )
                newUser.MailConfirmationCode=verificationCode;
           

            _userRepository.Add(newUser);
            int result = _userRepository.SaveChanges();

            //send email mehod impl
         
            return await Task.FromResult(result>0? _userRepository.GetByEmail(request.Email).UserId:0);

        }


        static bool IsPathValid(string path)
        {
            return Path.IsPathRooted(path) && (Path.GetInvalidPathChars().All(c => path.IndexOf(c) == -1));
        }

    }
}
