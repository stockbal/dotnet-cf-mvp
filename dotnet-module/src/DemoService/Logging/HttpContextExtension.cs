// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Logging;

public static class HttpContextExtension {
    private static readonly string CorrelationIdHeader = "x-correlation-id";

    public static string GetCorrelationId(this HttpContext context) {
        var correlation_id = context.Request.Headers[CorrelationIdHeader].ToString();
        if (string.IsNullOrEmpty(correlation_id)) {
            correlation_id = Guid.NewGuid().ToString();
            context.Request.Headers[CorrelationIdHeader] = correlation_id;
        }
        return correlation_id!;
    }
}
