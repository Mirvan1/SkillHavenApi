using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using SkillHaven.Shared.User.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SkillHaven.Application.Features.Users.Commands
{
    internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        public readonly IStringLocalizer _localizer;
        private readonly IMailService _mailService;


        public ResetPasswordCommandHandler(IUserService userService, IUserRepository userRepository, IMailService mailService)
        {
            _userService=userService;
            _userRepository=userRepository;
            _localizer=new Localizer();
            _mailService=mailService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token)
                || string.IsNullOrEmpty(request.Password)
                || string.IsNullOrEmpty(request.ConfirmPassword)) throw new UserVerifyException(_localizer["Conflict", "Errors", "Email"].Value);

            if (!request.Password.Equals(request.Password)) throw new UserVerifyException(_localizer["PasswordMatching","Errors"].Value);

            var user = _userService.GetUserFromToken(request.Token);

            if (user is null) throw new UserVerifyException("User not found");

            var getUser = _userRepository.GetByEmail(user.Email);

            if (!getUser.Email.Equals(request.Email)) throw new UserVerifyException(_localizer["Conflict", "Errors", "Email"].Value);

            if (getUser is null) throw new DatabaseValidationException(_localizer["UserNotFound", "Errors"].Value);

            getUser.Password=BCrypt.Net.BCrypt.HashPassword(request.Password);

            _userRepository.Update(getUser);
            int result = _userRepository.SaveChanges();

            string htmlConfirmMessage = File.ReadAllText(Directory.GetCurrentDirectory()+"/StaticFiles/reset-password-info-page.html");
            htmlConfirmMessage=htmlConfirmMessage.Replace("{ChangeDateTime}", DateTime.Now.ToLongDateString());


            if (result>0)
                await _mailService.SendEmail(new MailInfo()
                {
                    MailType=MailType.Html,
                  //  EmailBody="Your password successfuly changed",
                    EmailBody=htmlConfirmMessage,
                    EmailToId=getUser.Email,
                    EmailSubject="Password Change",
                    EmailToName=getUser.FirstName +" "+getUser.LastName
                });



            return  result>0;
        }
    }
}
