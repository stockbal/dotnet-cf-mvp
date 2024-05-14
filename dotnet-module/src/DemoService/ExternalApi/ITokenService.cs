using DevEpos.CF.Demo.Env;

namespace DevEpos.CF.Demo.ExternalApi {
    /// <summary>
    /// Service for retrieving OAuth tokens
    /// </summary>
    public interface ITokenService {
        Task<string> GetClientCredentialsToken(
            UaaCredentials credentials,
            params KeyValuePair<string, string>[] additionalTokenParams
        );
    }
}
