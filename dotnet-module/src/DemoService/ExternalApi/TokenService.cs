using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using DevEpos.CF.Demo.Common;
using DevEpos.CF.Demo.Env;

namespace DevEpos.CF.Demo.ExternalApi {
    class TokenService : ITokenService {
        private readonly IHttpClientFactory _clientFactory;

        public TokenService(IHttpClientFactory clientFactory) {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetClientCredentialsToken(
            UaaCredentials credentials,
            params KeyValuePair<string, string>[] additionalTokenParams
        ) {
            using var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.ConnectionClose = true;
            client.BaseAddress = new Uri(credentials.Url!);

            var tokenParams = new List<KeyValuePair<string, string>>() { new("grant_type", "client_credentials") };
            if (additionalTokenParams != null) {
                foreach (var param in additionalTokenParams) {
                    tokenParams.Add(param);
                }
            }

            // create request for reading a token
            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth/token") {
                Content = new FormUrlEncodedContent(tokenParams)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}")
                )
            );

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                var tokenJsonResponse = await response.Content.ReadAsStringAsync();
                var parsedTokenJson = JsonNode.Parse(tokenJsonResponse);
                return (parsedTokenJson?["access_token"]?.GetValue<string>())!;
            } else {
                string? errorBody = null;
                if (response.Content != null) {
                    errorBody = await response.Content.ReadAsStringAsync();
                }
                throw new ServiceException(
                    (int)response.StatusCode,
                    "Token Fetch Failed",
                    $"url: {credentials.Url}",
                    errorBody
                );
            }
        }

    }
}
