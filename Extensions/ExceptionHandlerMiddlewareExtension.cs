using Microsoft.AspNetCore.Diagnostics;

namespace PersonalFinanceTracker.Extensions;

public static class ExceptionHandlerMiddlewareExtension
{
    public static void UseExceptionHandlerExtended(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var error = context.Features.Get<IExceptionHandlerFeature>();
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                
                var statusCode = error?.Error switch
                {
                    InvalidOperationException => 409,
                    UnauthorizedAccessException => 401,
                    ArgumentException => 400,
                    _ => 500
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                
                logger?.LogError(error?.Error, "Unhandled exception occurred");

                var response = new
                {
                    StatusCode = statusCode,
                    Message = error?.Error.Message ?? "An error occurred",
                    Details = app.Environment.IsDevelopment() ? error?.Error.StackTrace : null
                };

                await context.Response.WriteAsJsonAsync(response);
            });
        });
    }
}