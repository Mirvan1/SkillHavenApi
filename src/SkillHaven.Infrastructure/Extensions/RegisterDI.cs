using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Infrastructure.Data;
using SkillHaven.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Infrastructure.Extensions;

public static class RegisterDI
{
    public static IServiceCollection AddDIRegistration(this IServiceCollection services)
    {


       services.AddScoped<IUserRepository, UserRepository>();
       services.AddScoped<ISupervisorRepository, SupervisorRepository>();
       services.AddScoped<IBlogRepository, BlogRepository>();
       services.AddScoped<IConsultantRepository, ConsultantRepository>();
       services.AddScoped<IUserService, UserService>();
       services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
       services.AddScoped<IMessageRepository, MessageRepository>();
       services.AddScoped<IChatUserRepository, ChatUserRepository>();
       services.AddScoped<IUserConnectionRepository, ChatUserConnectionRepository>();
       services.AddScoped<IMailService, MailService>();
       services.AddScoped<IUtilService, UtilService>();
       services.AddScoped<IBlogVoteRepository, BlogVoteRepository>();
       services.AddScoped<IBlogTopicRepository, BlogTopicRepository>();
       services.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));//init only once 
        return services;
    }

    public static IServiceCollection AddDBContext(this IServiceCollection services,IConfiguration configuration,NLog.Logger logger)
    {
        services.AddDbContext<shDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
              .LogTo(logger.Warn, Microsoft.Extensions.Logging.LogLevel.Warning));
        return services;
    }

}

