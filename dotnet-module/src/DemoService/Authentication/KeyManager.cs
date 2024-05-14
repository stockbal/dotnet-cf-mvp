using System.Security;
using System.Security.Cryptography;
using System.Text;
using DevEpos.CF.Demo.Env;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DevEpos.CF.Demo.Authentication {
    public class KeyManager(IServiceEnv env, IHttpClientFactory clientFactory) : IKeyManager {
        private const string Protocol = "https";
        private static readonly TimeSpan DefaultExpirationTime = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan DefaultRefreshPeriod = TimeSpan.FromMinutes(15);
        private readonly IServiceEnv _env = env;
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private SecurityKey? _key;
        private DateTime? _keyFetchedAt;

        public SecurityKey GetSecurityKey(string kid, SecurityToken token) {
            var jwtToken = (JsonWebToken)token;

            if (_key == null || (DateTime.Now - _keyFetchedAt) >= DefaultExpirationTime) {
                RefreshKey(kid, jwtToken);
            } else if ((DateTime.Now - _keyFetchedAt) > DefaultRefreshPeriod) {
                Task.Run(() => RefreshKey(kid, jwtToken));
            }

            return _key;
        }

        private void RefreshKey(string kid, JsonWebToken jwtToken) {
            var creds = _env.XsuaaCredentials.First();
            var uaaDomain = creds.UaaDomain!;
            if (!uaaDomain.StartsWith(Protocol)) {
                uaaDomain = $"{Protocol}://{uaaDomain}";
            }

            var jwks = FetchJwks($"{uaaDomain}/token_keys", creds.ZoneId!);

            var jwk = jwks.Keys.First(k => k.KeyId == kid);
            if (jwk == null) {
                throw new SecurityException();
            }
            _key = CreateKey(jwk.KeyId, jwk.AdditionalData["value"].ToString()!);
            _keyFetchedAt = DateTime.Now;
        }

        /// <summary>
        /// Fetches the Json Web Keys for the given domain and zone
        /// </summary>
        private JsonWebKeySet FetchJwks(string url, string zoneId) {
            using var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                QueryHelpers.AddQueryString(url, new Dictionary<string, string?>() {
                    ["zid"] = zoneId
                })
            );
            var response = client.Send(request);
            var contentReader = new StreamReader(response.Content.ReadAsStream(), Encoding.UTF8);
            var jsonString = contentReader.ReadToEnd();

            return new JsonWebKeySet(jsonString);
        }

        private SecurityKey CreateKey(string id, string key) {
            var rsa = RSA.Create();
            rsa.ImportFromPem(key);
            return new RsaSecurityKey(rsa) {
                KeyId = id
            };
        }
    }
}
