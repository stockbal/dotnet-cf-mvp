using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace DevEpos.CF.Demo.Authentication {
    /// <summary>
    /// Dummy JWT options
    /// </summary>
    public class DummyJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions> {
        public void Configure(JwtBearerOptions o) { }
        public void Configure(string? name, JwtBearerOptions o) { }
    }
}
