// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Logging;

public static class Context {
    public static string CorrelationId { get; set; } = string.Empty;

    public static void Start() {
        CorrelationId = Guid.NewGuid().ToString();
    }

    public static void End() {
        CorrelationId = string.Empty;
    }
}
