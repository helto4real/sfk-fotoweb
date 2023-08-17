using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Infrastructure.ExceptionsHandling;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);

            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new ErrorDetail
        {
            Title = GetTitle(exception),
            StatusCode = statusCode,
            Detail = exception.Message,
            Errors = GetErrors(exception)
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            ForbiddenException =>StatusCodes.Status403Forbidden,
            UnAuthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            NotFoundException => "Not found",
            ValidationException => "Validation error",
            BadRequestException => "Bad request",
            ForbiddenException => "Forbidden",
            UnAuthorizedException => "Unauthorized",
            _ => "Server Error"
        };

    private static IEnumerable<ValidationFailure>? GetErrors(Exception exception)
    {
        IEnumerable<ValidationFailure>? errors = null;

        if (exception is ValidationException validationException)
        {
            errors = validationException.Errors;
        }

        return errors;
    }
}

public class ErrorDetail
{
    public string Title { get; set; } = default!;
    public int StatusCode { get; set; }
    public string Detail { get; set; } = default!;
    public IEnumerable<ValidationFailure>? Errors { get; set; }
}