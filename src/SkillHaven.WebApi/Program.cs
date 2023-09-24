using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillHaven.Application.Features.Users.Queries;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Application.Mappings;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using SkillHaven.Infrastructure.Repositories;
using SkillHaven.Shared;
using SkillHaven.WebApi.Controllers;
using Swashbuckle.AspNetCore.Filters;
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

builder.Services.AddDbContext<shDbContext>(options =>
              options.UseSqlServer("Data Source=DESKTOP-HGKR5P9\\MSSQLSERVERDB;Initial Catalog=SkillHavenDB;Integrated Security=True;Encrypt=false;"));
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

    options.OperationFilter<SecurityRequirementsOperationFilter>();


});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://localhost:8000", "http://127.0.0.1:8080") // adjust the port if you use a different one
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

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
