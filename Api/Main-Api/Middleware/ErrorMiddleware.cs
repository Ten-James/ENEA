using Api.BussinesLogic.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Main_Api.Middleware;

public class ErrorMiddleware
{
    private readonly ILogger<ErrorMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError("Entity not found: {Id}", ex.Id);
            await HandleProblem(context, new ProblemDetails { Status = (int)HttpStatusCode.NotFound, Title = "Entity not found", Detail = ex.Message, Instance = context.Request.Path });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleProblem(HttpContext context, ProblemDetails problemDetails)
    {

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails { Status = (int)HttpStatusCode.InternalServerError, Title = "An error occurred while processing your request.", Detail = exception.Message, Instance = context.Request.Path };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}