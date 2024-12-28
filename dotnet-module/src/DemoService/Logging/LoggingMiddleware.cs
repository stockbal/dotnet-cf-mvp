// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Logging;

public class LoggingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public LoggingMiddleware(
        RequestDelegate next,
        ILogger<LoggingMiddleware> logger
    ) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext) {
        var state = new Dictionary<string, object> {
            [ICfLoggingProps.CorrelationId] = httpContext.GetCorrelationId()
        };

        using (_logger.BeginScope(state)) {
            await _next(httpContext);
        }
    }
}
