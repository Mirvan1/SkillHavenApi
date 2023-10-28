using MediatR;
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
using SkillHaven.Shared;
using SkillHaven.WebApi.Extensions;
using SkillHaven.WebApi.Hubs;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
var connStr=configuration.GetConnectionString("DefaultConnection");


var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

logger.Error("ssss");
builder.Host.UseNLog();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Logging.AddNLog();




builder.Services.AddDbContext<shDbContext>(options =>
              options.UseSqlServer("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=SkillHavenDB;Integrated Security=True;Encrypt=false;")
              .LogTo(logger.Warn,Microsoft.Extensions.Logging.LogLevel.Warning));
var assm = Assembly.GetExecutingAssembly();


builder.Services.AddHttpContextAccessor();//httpcontext accesor di

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();

builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IConsultantRepository, ConsultantRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatUserRepository, ChatUserRepository>();
builder.Services.AddScoped<IUserConnectionRepository, ChatUserConnectionRepository>();

builder.Services.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));//init only once 

//builder.Services.AddScoped<IStringLocalizer>(sp => new Localizer("Errors"));


//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
var ss = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("SkillHaven")).ToArray();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(ss );
    
});

//builder.Services.AddMediatR(assm);

//builder.Services.AddMediatR(cfg =>
//     cfg.RegisterServicesFromAssembly(typeof(GetUserQueryHandler).Assembly));
builder.Services.AddAutoMapper(ss);


builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters=new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey=true,
        ValidateAudience=false,
        ValidateIssuer=false,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["secret-key"]))
        
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
        builder => builder.WithOrigins("http://localhost:8000", "http://127.0.0.1:8080") 
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



//var logger = NLog.LogManager.Setup().LoadConfigurationFromXml("nlog.config").GetCurrentClassLogger();


var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var logger2 = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
logger2.LogInformation("This should log from the Program class.");

// Configure the HTTP request pipeline.
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
