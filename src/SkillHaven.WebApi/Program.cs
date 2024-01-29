using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using SkillHaven.Application.Features.Users.Queries;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Application.Mappings;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using SkillHaven.Infrastructure.Repositories;
using SkillHaven.Shared.User.Mail;
using SkillHaven.WebApi.Extensions;
using SkillHaven.WebApi.Hubs;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using FluentValidation.AspNetCore;
using SkillHaven.Shared.User;
using SkillHaven.Infrastructure.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;


var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllers();

builder.Services.AddFluentValidation(fv => {
    fv.RegisterValidatorsFromAssemblyContaining<Program>();
    var assembly = Assembly.Load("SkillHaven.Application");

    fv.RegisterValidatorsFromAssembly(assembly); 
});
  
builder.Host.UseNLog();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddNLog();

builder.Services.AddDBContext(builder.Configuration, LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger());


 

builder.Services.AddHttpContextAccessor(); 
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<SkillRater>(builder.Configuration.GetSection("SkilRater"));
builder.Services.AddDIRegistration();

 

 var assm = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("SkillHaven")).ToArray();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assm );
    
});

//builder.Services.AddMediatR(assm);

//builder.Services.AddMediatR(cfg =>
//     cfg.RegisterServicesFromAssembly(typeof(GetUserQueryHandler).Assembly));
builder.Services.AddAutoMapper(assm);

builder.Services.AddAuth(builder.Configuration);
 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standart Authorize header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<AcceptLanguageOperationFilter>();
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
 builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                //builder => builder.WithOrigins("http://localhost:8000", "http://localhost:4200", "http://127.0.0.1:8080") 
         opt => opt.WithOrigins(builder.Configuration.GetSection("CorsPolicy:AllowedOrigins").Value.Split(","))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-EN", "tr-TR" };

    options.SetDefaultCulture(supportedCultures[0])
          .AddSupportedCultures(supportedCultures)
          .AddSupportedUICultures(supportedCultures);

    options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new QueryStringRequestCultureProvider(),
            new AcceptLanguageHeaderRequestCultureProvider()
        };

});

builder.Services.AddSignalR();


var app = builder.Build();

//using var serviceScope = app.Services.CreateScope();
//var logger2 = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//logger2.LogInformation("This should log from the Program class.");

 if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("CorsPolicy");
var localizeOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizeOptions.Value);

app.UseMiddleware<ExceptionMiddleware>();
 
app.UseRouting();  

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");  // SignalR Hub mapping
    endpoints.MapControllers();  // Map controller routes
});


app.Run();
