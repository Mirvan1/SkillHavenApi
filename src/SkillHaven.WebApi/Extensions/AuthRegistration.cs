using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SkillHaven.WebApi.Extensions;

public static class AuthRegistration
{
    public static IServiceCollection AddAuth(this IServiceCollection services,IConfiguration configuration)
    {
        string secretKey = configuration["secret-key"].Replace("{secret-key-value}",
     Environment.GetEnvironmentVariable("secret-key-value", EnvironmentVariableTarget.User));

        var key = Encoding.ASCII.GetBytes(secretKey);
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken) && context.Request.Path.StartsWithSegments("/chatHub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        return services;
    }
}
