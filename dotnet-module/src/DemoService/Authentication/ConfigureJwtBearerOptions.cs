using DevEpos.CF.Demo.Env;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevEpos.CF.Demo.Authentication {
    /// <summary>
    /// JWT bearer options for authentication configuration
    /// </summary>
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions> {
        private readonly IKeyManager _keyManager;
        private readonly IServiceEnv _env;

        public ConfigureJwtBearerOptions(IKeyManager keyManager, IServiceEnv env) {
            _keyManager = keyManager;
            _env = env;
        }

        public void Configure(JwtBearerOptions o) {
            o.TokenValidationParameters = new TokenValidationParameters {
                ValidIssuer = $"{_env.XsuaaCredentials.First().Url}/oauth/token",
                ValidAudience = _env.XsuaaCredentials.First().XsAppName,
                ValidateLifetime = true,
                IssuerSigningKeyResolver = (string t, SecurityToken securityToken, string kid, TokenValidationParameters tokenParams) =>
                    [_keyManager.GetSecurityKey(kid, securityToken)],
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };
        }

        public void Configure(string? name, JwtBearerOptions o) {
            Configure(o);
        }
    }
}
