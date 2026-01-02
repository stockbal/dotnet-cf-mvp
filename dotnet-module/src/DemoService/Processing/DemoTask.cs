// Copyright (c) Demo AG. All Rights Reserved.

using System.Text.Json.Serialization;

namespace DevEpos.CF.Demo.Processing;

public class DemoTask {
    public string? ID { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("delay")]
    public int? Delay { get; set; }
}
