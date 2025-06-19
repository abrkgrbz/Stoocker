using Azure;
using System.Net;
using System.Text.Json;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Exceptions;
using System.Text.Json.Serialization;

namespace Stoocker.API.Middlewares
{
    public class ErrorResponse
    {
        public bool IsSuccess { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "An error occurred: {Message}", error.Message);

                var response = context.Response;
                response.ContentType = "application/json";

                var errorResponse = new ErrorResponse();

                switch (error)
                {
                      
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorResponse.ErrorMessage = "Validation failed";
                        errorResponse.ErrorCode = "VALIDATION_ERROR";
                        errorResponse.ValidationErrors = e.Errors
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value.ToArray()
                            );
                        break;

                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        errorResponse.ErrorMessage = e.Message ?? "Resource not found";
                        errorResponse.ErrorCode = "NOT_FOUND";
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse.ErrorMessage = "An unexpected error occurred";
                        errorResponse.ErrorCode = "INTERNAL_ERROR";
                        break;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var result = JsonSerializer.Serialize(errorResponse, options);
                await response.WriteAsync(result);
            }
        }
    }
}
