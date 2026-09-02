// Copyright (c) Demo AG. All Rights Reserved.

using System.Net.Http.Headers;
using DevEpos.CF.Demo.EnvAccess;
using DevEpos.CF.Demo.ExternalApi;

namespace DevEpos.CF.Demo.Services;

public class ShutdownService : IHostedService {
    private readonly ILogger<ShutdownService> _logger;
    private readonly IServiceEnv _env;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _clientFactory;


    public ShutdownService(ILogger<ShutdownService> logger, IHttpClientFactory clientFactory, IServiceEnv env, ITokenService tokenService) {
        _logger = logger;
        _clientFactory = clientFactory;
        _env = env;
        _tokenService = tokenService;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Host is stopping - cleanup running...");

        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.ConnectionClose = true;
        client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CAP_SRV_URL")!);

        var token = await _tokenService.GetClientCredentialsToken(_env.XsuaaCredentials[0]);

        var request = new HttpRequestMessage(HttpMethod.Post, "/odata/v4/queue/cancelTasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("correlation_id", Guid.NewGuid().ToString());

        var content = new StringContent($"{{\"appIndex\":{int.Parse(Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX")!)}}}");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;

        await client.SendAsync(request, cancellationToken);
    }
}
