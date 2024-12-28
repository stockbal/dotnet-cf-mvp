// Copyright (c) Demo AG. All Rights Reserved.

using Serilog.Core;
using Serilog.Events;

namespace DevEpos.CF.Demo.Logging;

public class LoggerTypeEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher {
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf) {
        // use SourceContext as logger property
        if (logEvent.Properties.ContainsKey("SourceContext")) {
            var sourceContext = logEvent.Properties["SourceContext"].ToString().Trim('"');
            logEvent.AddPropertyIfAbsent(pf.CreateProperty(ICfLoggingProps.Logger, sourceContext));
        }

        // add correlation id
        if (_httpContextAccessor.HttpContext != null) {
            logEvent.AddPropertyIfAbsent(pf.CreateProperty(ICfLoggingProps.CorrelationId, _httpContextAccessor.HttpContext.GetCorrelationId()));
        }
    }
}
