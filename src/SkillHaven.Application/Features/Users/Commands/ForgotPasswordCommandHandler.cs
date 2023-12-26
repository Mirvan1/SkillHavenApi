using MediatR;
using Microsoft.Extensions.Configuration;
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
using System.Web;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;
        public readonly IStringLocalizer _localizer;
        private readonly IConfiguration _configuration;


        public ForgotPasswordCommandHandler(IUserRepository userRepository, IMailService mailService, IUserService userService, IConfiguration configuration)
        {
            _userRepository=userRepository;
            _mailService=mailService;
            _userService=userService;
            _localizer=new Localizer();
            _configuration=configuration;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email)) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var getUser = _userRepository.GetByEmail(request.Email);

            if (getUser is null) throw new DatabaseValidationException(_localizer["UnAuthorized", "Errors"].Value);

            string token = _userService.CreateToken(getUser);
            string clientEndpoint=_configuration["clienturl-forgot-password"];

            var queryParam=HttpUtility.ParseQueryString(string.Empty);
            queryParam.Add("email",getUser.Email);
            queryParam.Add("value", token);

            await _mailService.SendEmail(new Shared.User.Mail.MailInfo()
            {
                MailType=MailType.PlainText,
                EmailBody=$"{clientEndpoint}?{queryParam}",
                EmailToId=getUser.Email,
                EmailSubject="Forgot Password ",
                EmailToName=getUser.FirstName +" "+getUser.LastName
            });

            return await Task.FromResult(true);

        }
    }
}
