using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Services.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Logging
{
    public class PerformanceLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<PerformanceLoggingMiddleware> _logger;
        private readonly ITenantAwareLogger _tenantLogger;

        public PerformanceLoggingMiddleware(
            ILogger<PerformanceLoggingMiddleware> logger,
            ITenantAwareLogger tenantLogger)
        {
            _logger = logger;
            _tenantLogger = tenantLogger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            { 
                await LogRequest(context);
                 
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;
                 
                await next(context); 
                await LogResponse(context, responseBody, originalBodyStream, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError(context, ex, stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequest(HttpContext context)
        { 
            if (IsSensitiveEndpoint(context.Request.Path))
                return;

            var request = context.Request;
            var requestInfo = new StringBuilder();
            requestInfo.AppendLine($"HTTP Request Information:");
            requestInfo.AppendLine($"Schema: {request.Scheme}");
            requestInfo.AppendLine($"Host: {request.Host}");
            requestInfo.AppendLine($"Path: {request.Path}");
            requestInfo.AppendLine($"QueryString: {request.QueryString}");
            requestInfo.AppendLine($"Method: {request.Method}");
            requestInfo.AppendLine($"ContentType: {request.ContentType}");
             
            requestInfo.AppendLine("Headers:");
            foreach (var header in request.Headers.Where(h => !IsSensitiveHeader(h.Key)))
            {
                requestInfo.AppendLine($"  {header.Key}: {header.Value}");
            }

            _tenantLogger.LogDebug(requestInfo.ToString());
             
            if ((request.Method == HttpMethods.Post || request.Method == HttpMethods.Put)
                && request.ContentLength > 0
                && request.ContentLength < 10240) // 10KB limit
            {
                request.EnableBuffering();
                var body = await ReadRequestBody(request);
                if (!string.IsNullOrEmpty(body))
                {
                    _tenantLogger.LogDebug($"Request Body: {body}");
                }
            }
        }

        private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream, long elapsedMs)
        {
            var response = context.Response;
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
             
            await responseBody.CopyToAsync(originalBodyStream);

            var responseInfo = new StringBuilder();
            responseInfo.AppendLine($"HTTP Response Information:");
            responseInfo.AppendLine($"StatusCode: {response.StatusCode}");
            responseInfo.AppendLine($"ContentType: {response.ContentType}");
            responseInfo.AppendLine($"ContentLength: {response.ContentLength ?? responseBody.Length}");
            responseInfo.AppendLine($"Duration: {elapsedMs}ms");
             
            if (elapsedMs > 1000)
            {
                _tenantLogger.LogWarning($"Slow request detected: {context.Request.Method} {context.Request.Path} took {elapsedMs}ms");
            }
            else
            {
                _tenantLogger.LogInformation($"Request completed: {context.Request.Method} {context.Request.Path} - {response.StatusCode} ({elapsedMs}ms)");
            }
             
            if (_logger.IsEnabled(LogLevel.Debug) && responseBody.Length < 10240)
            {
                _tenantLogger.LogDebug($"Response Body: {responseText}");
            }
        }

        private void LogError(HttpContext context, Exception ex, long elapsedMs)
        {
            _tenantLogger.LogError(ex,
                $"Request failed: {context.Request.Method} {context.Request.Path} after {elapsedMs}ms");
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private bool IsSensitiveEndpoint(PathString path)
        {
            var sensitiveEndpoints = new[] { "/login", "/register", "/token", "/password" };
            return sensitiveEndpoints.Any(endpoint =>
                path.Value?.Contains(endpoint, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private bool IsSensitiveHeader(string headerKey)
        {
            var sensitiveHeaders = new[] { "Authorization", "Cookie", "X-Api-Key" };
            return sensitiveHeaders.Contains(headerKey, StringComparer.OrdinalIgnoreCase);
        }
    }
     
    public static class PerformanceLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceLoggingMiddleware>();
        }
    }
}
