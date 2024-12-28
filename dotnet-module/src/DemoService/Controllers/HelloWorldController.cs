// Copyright (c) Demo AG. All Rights Reserved.

using DevEpos.CF.Demo.Authentication;
using DevEpos.CF.Demo.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace DevEpos.CF.Demo.Controllers;

[ApiController]
[Route("[controller]", Name = "Hello World")]
public class HelloWorldController(ILogger<HelloWorldController> logger, SomeProcessor processor) : ControllerBase {
    private readonly ILogger<HelloWorldController> _logger = logger;
    private readonly SomeProcessor _processor = processor;

    /// <summary>
    /// Simple Hello World Endpoint
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [Authorize(Policy = IIdentityData.DefaultAuthPolicyName)]
    [Authorize(Policy = IIdentityData.UserPolicyName)]
    public string PrintHelloWorld() {
        _logger.LogInformation("Before returning hello world");
        try {
            _processor.ProcessData();
        } catch (Exception e) {
            using (LogContext.PushProperty("stacktrace", e.StackTrace.Split("\n"))) {
                _logger.LogError("Failed to process data {error}", e.Message);
            }
        }
        return "Hello World";
    }

}
