﻿using DevEpos.CF.Demo.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevEpos.CF.Demo.Controllers;

[ApiController]
[Route("[controller]", Name = "Hello World")]
public class HelloWorldController() : ControllerBase {
    /// <summary>
    /// Simple Hello World Endpoint
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [Authorize(Policy = IIdentityData.DefaultAuthPolicyName)]
    [Authorize(Policy = IIdentityData.UserPolicyName)]
    public string PrintHelloWorld() {
        return "Hello World";
    }

}
