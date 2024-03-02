using MediatR;
using Microsoft.Extensions.Configuration;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Queries
{
    public class GetCaptchaUIKeyQueryHandler : IRequestHandler<GetCaptchaUIKeyQuery, string>
    {
        private readonly IConfiguration _configuration;

        public GetCaptchaUIKeyQueryHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Handle(GetCaptchaUIKeyQuery request, CancellationToken cancellationToken)
        {
            var captchaUiKey = _configuration["captcha-ui-key"].Replace("{captcha-ui-key-value}", Environment.GetEnvironmentVariable("captcha-ui-key-value", EnvironmentVariableTarget.User));
            return await Task.FromResult(captchaUiKey);
        }
    }
}
