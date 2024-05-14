using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevEpos.CF.Demo.Env {
    # region private classes for JSON parsing

    class VcapEnvironment {
        [JsonRequired]
        public List<XsuaaBinding>? xsuaa { get; set; }
    }
    class XsuaaBinding {
        [JsonRequired]
        public UaaCredentials? credentials { get; set; }

        [JsonRequired]
        public string? plan { get; set; }
    }

    # endregion

    /// <summary>
    /// Access to service credentials via parsing environment variable
    /// "VCAP_SERVICES"
    /// </summary>
    public class CfServiceEnv : IServiceEnv {
        private const string VcapServicesEnvVar = "VCAP_SERVICES";
        private readonly List<UaaCredentials> _xsuaaCreds;

        public CfServiceEnv() {
            var json = Environment.GetEnvironmentVariable(VcapServicesEnvVar);
            if (json != null) {
                try {
                    var environment = JsonSerializer.Deserialize<VcapEnvironment>(json)!;
                    _xsuaaCreds = environment?.xsuaa!.Select(x => x.credentials).ToList()!;
                } catch (Exception) {
                    throw new Exception($"Parsing of {VcapServicesEnvVar} failed");
                }
            } else {
                throw new Exception($"Environment variable {VcapServicesEnvVar} is empty");
            }
        }

        /// <summary>
        /// Access Credentials of bound UAA instance
        /// </summary>
        public List<UaaCredentials> XsuaaCredentials => _xsuaaCreds;
    }
}
