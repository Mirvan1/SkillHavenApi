using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using SkillHaven.Shared.User.Mail;
using SkillHaven.WebApi.Extensions;
using SkillHaven.WebApi.Hubs;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using FluentValidation.AspNetCore;
using SkillHaven.Shared.User;
using SkillHaven.Infrastructure.Extensions;

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
builder.Services.AddSignalR(e =>{
    e.MaximumReceiveMessageSize = 102400000;
});

var app = builder.Build();
if (Convert.ToBoolean(builder.Configuration["ShowSwagger"].ToString()))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value);
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
