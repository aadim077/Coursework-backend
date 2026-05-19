using coursework.Models;
using System.Net;

namespace coursework.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // You can customize the response based on the exception type here
        // For example, if (exception is NotFoundException) return 404, etc.

        var response = new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = "An internal server error occurred. Please try again later."
        };

        // Optionally, include exception details in development environment
        // var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        // if (environment.IsDevelopment())
        // {
        //     response.Message = exception.Message;
        // }

        await context.Response.WriteAsync(response.ToString());
    }
}
