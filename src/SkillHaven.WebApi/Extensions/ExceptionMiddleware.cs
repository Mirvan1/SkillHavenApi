using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Exceptions;
using System.Net;

namespace SkillHaven.WebApi.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next
        , ILoggerService<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger=logger;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        { 
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(new ErrorDto()
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error",
            DetailMessage=exception.Message
        }.ToString()); 
    }

}
