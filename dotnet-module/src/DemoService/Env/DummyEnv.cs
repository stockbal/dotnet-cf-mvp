namespace DevEpos.CF.Demo.Env {
    /// <summary>
    /// Dummy service env variables
    /// </summary>
    public class DummyEnv : IServiceEnv {

        private readonly List<UaaCredentials> _xsuaaCredentials;

        public DummyEnv() {
            _xsuaaCredentials = new List<UaaCredentials>{
                new() {
                    ClientId = "sb-client-dotnet-demo",
                    ClientSecret = "secret",
                    Url = "https://auth.com",
                    XsAppName = "dotnet-demo",
                    VerificationKey = "BEGIN...END",
                    UaaDomain = "authentication.com"
                },
                new() {
                    ClientId = "sb-client-dotnet-demo2",
                    ClientSecret = "secret",
                    Url = "https://auth.com",
                    XsAppName = "dotnet-demo",
                    VerificationKey = "BEGIN...END",
                    UaaDomain = "authentication.com"
                }
            };
        }

        public List<UaaCredentials> XsuaaCredentials => _xsuaaCredentials;
    }
}
