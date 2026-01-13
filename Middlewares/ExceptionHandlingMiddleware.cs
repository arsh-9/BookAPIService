using BookAPIService.Exceptions;
using BookAPIService.Models.Errors;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                new ApiError
                {
                    Title = "Invalid request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                }),

            ExternalServiceException ex => (    
                StatusCodes.Status502BadGateway,
                new ApiError
                {
                    Title = "External service error",
                    Status = StatusCodes.Status502BadGateway,
                    Detail = ex.Message
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ApiError
                {
                    Title = "Internal server error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred"
                })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(error);
    }
}