// Copyright (c) Demo AG. All Rights Reserved.

using DevEpos.CF.Demo.Env;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DevEpos.CF.Demo.Authentication;

/// <summary>
/// Options for authorization configuration
/// </summary>
public class ConfigureAuthorizationOptions : IConfigureNamedOptions<AuthorizationOptions> {

    private readonly IServiceEnv _env;
    private const string ScopeClaim = "scope";
    private const string ZoneIdClaim = "zid";

    public ConfigureAuthorizationOptions(IServiceEnv env) {
        _env = env;
    }

    public void Configure(AuthorizationOptions options) {
        if (_env.XsuaaCredentials == null) {
            throw new Exception("XSUAA Credentials not available");
        }
        options.AddPolicy(IIdentityData.DefaultAuthPolicyName, p => {
            p.RequireClaim(ZoneIdClaim, _env.XsuaaCredentials.First().ZoneId!);
        });

        // add policy to check scopes defined in /xs-security.json
        options.AddPolicy(IIdentityData.UserPolicyName, p => {
            p.RequireClaim(
                ScopeClaim,
                $"{_env.XsuaaCredentials.First().XsAppName}.{IIdentityData.UserScopeValue}"
            );
        });
    }


    public void Configure(string? name, AuthorizationOptions options) {
        Configure(options);
    }
}
