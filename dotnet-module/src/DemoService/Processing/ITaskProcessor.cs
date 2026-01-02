// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Processing;

public interface ITaskProcessor {
    Task<int> ProcessTaskAsync(CancellationToken cancellationToken);

    Task CancelProcessingTasks(CancellationToken cancellationToken);
}
