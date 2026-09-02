// Copyright (c) Demo AG. All Rights Reserved.

using System.Net.Http.Headers;
using System.Text.Json;
using DevEpos.CF.Demo.EnvAccess;
using DevEpos.CF.Demo.ExternalApi;
using DevEpos.CF.Demo.Logging;

namespace DevEpos.CF.Demo.Processing;

public class TaskProcessor : ITaskProcessor {
    private readonly ILogger<ITaskProcessor> _logger;
    private readonly IServiceEnv _env;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _clientFactory;

    public TaskProcessor(ILogger<ITaskProcessor> logger, IHttpClientFactory clientFactory, IServiceEnv env, ITokenService tokenService) {
        _logger = logger;
        _clientFactory = clientFactory;
        _env = env;
        _tokenService = tokenService;
    }

    public async Task<int> ProcessTaskAsync(CancellationToken cancellationToken) {
        // fetch next open task
        var task = await FetchNextOpenTaskAsync(cancellationToken);
        if (task?.ID != null) {
            _logger.LogInformation("Processing Task with ID: {taskId} and Name: {taskName}", task.ID, task.Name);
            // Simulate work
            var randomSeconds = task.Delay != null && task.Delay > -1 ? task.Delay : Random.Shared.Next(30, 101);
            _logger.LogInformation("Simulating work for {seconds} seconds", randomSeconds);
            await Task.Delay(TimeSpan.FromSeconds((int)randomSeconds), cancellationToken);

            // set task to completed
            if (await SetTaskCompleted(task.ID, cancellationToken)) {
                _logger.LogInformation("Task with ID: {taskId} has been completed.", task.ID);
            } else {
                _logger.LogWarning("Failed to complete Task with ID: {taskId}.", task.ID);
            }
            return 1;
        }
        return 0;
    }

    private async Task<DemoTask?> FetchNextOpenTaskAsync(CancellationToken cancellationToken) {
        using var client = CreateConfiguredHttpClient();

        _logger.LogInformation("Reserving next open task from queue...");
        var request = await CreateRequestWithAuthenticationAsync(HttpMethod.Post, "/odata/v4/queue/reserveOpenTask",
            $"{{\"instanceIndex\":{int.Parse(Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX")!)}}}");

        var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode) {
            var jsonResult = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received open task: {jsonResult}", jsonResult);
            if (string.IsNullOrEmpty(jsonResult)) {
                return null;
            }
            return JsonSerializer.Deserialize<DemoTask>(jsonResult);
        } else {
            _logger.LogError("Failed to reserve open task. Status Code: {statusCode}, Body: {body}", response.StatusCode, response.Content?.ToString());
        }
        return null;
    }

    private async Task<bool> SetTaskCompleted(string taskId, CancellationToken cancellationToken) {
        using var client = CreateConfiguredHttpClient();

        var request = await CreateRequestWithAuthenticationAsync(HttpMethod.Post, $"/odata/v4/queue/Tasks({taskId})/complete");

        var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode) {
            return true;
        }
        return false;
    }

    private HttpClient CreateConfiguredHttpClient() {
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.ConnectionClose = true;
        client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CAP_SRV_URL")!);
        return client;
    }

    private async Task<HttpRequestMessage> CreateRequestWithAuthenticationAsync(HttpMethod method, string path, string jsonContent = "{}") {
        var token = await _tokenService.GetClientCredentialsToken(_env.XsuaaCredentials[0]);

        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("correlation_id", Context.CorrelationId);

        var content = new StringContent(jsonContent);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;

        return request;
    }

    public async Task CancelProcessingTasks(CancellationToken cancellationToken) {
        using var client = CreateConfiguredHttpClient();
        var instanceIndex = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");

        _logger.LogInformation("Cancelling all tasks that are processed by instance {index}", instanceIndex);
        var request = await CreateRequestWithAuthenticationAsync(HttpMethod.Post, "/odata/v4/queue/reserveOpenTask",
            $"{{\"instanceIndex\":{int.Parse(instanceIndex!)}}}");

        await client.SendAsync(request, cancellationToken);
    }
}
