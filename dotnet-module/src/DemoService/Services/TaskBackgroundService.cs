// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Services;

using DevEpos.CF.Demo.Logging;
using DevEpos.CF.Demo.Processing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class TaskBackgroundService : BackgroundService {
    private readonly ILogger<TaskBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public TaskBackgroundService(ILogger<TaskBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("TaskBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested) {
            Context.CorrelationId = Guid.NewGuid().ToString();
            using (_logger.BeginScope(new Dictionary<string, object> { { "correlation_id", Context.CorrelationId } })) {
                _logger.LogInformation("Background task running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceScopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<ITaskProcessor>();

                try {
                    if (await processor.ProcessTaskAsync() == 0) {
                        _logger.LogInformation("No tasks to process. Waiting before next check...");
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error occurred executing background task.");
                }
            }
            Context.CorrelationId = string.Empty;
        }

        _logger.LogInformation("TaskBackgroundService is stopping.");
    }
}
