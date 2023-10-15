using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Infrastructure.Exceptions;
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

        public RegisterUserCommandHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository=userRepository;
            _configuration = configuration;

        }

        public Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var checkEmail = _userRepository.GetByEmail(request.Email);//buraya get by email eklenecek;

            if (checkEmail is not null)
                throw new DatabaseValidationException("This email already registered");

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
                ProfilePicture=request.ProfilePicture,
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

            return Task.FromResult(result>0);

        }


        static bool IsPathValid(string path)
        {
            return Path.IsPathRooted(path) && (Path.GetInvalidPathChars().All(c => path.IndexOf(c) == -1));
        }

        private void SavePhoto(string photoBase64, string userName)
        {
            string imagesFolderPath = _configuration["ImagesFolderPath"];
            byte[] imageBytes = Convert.FromBase64String(photoBase64);

            string fileName = userName+"_"+DateTime.Now;
            string filePath = Path.Combine(imagesFolderPath, fileName);

            File.WriteAllBytes(filePath, imageBytes);
        }
    }
}
