// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Common;

public class SomeProcessor(ILogger<SomeProcessor> logger) {
    private readonly ILogger _logger = logger;

    public void ProcessData() {
        _logger.LogInformation("Inside Processor");
        _logger.LogWarning("Processor produced warning");
        _logger.LogCritical("Some critical message");
        throw new Exception("Some Error");
    }
}
