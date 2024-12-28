// Copyright (c) Demo AG. All Rights Reserved.

using System.Text.Json.Serialization;

namespace DevEpos.CF.Demo.Env;

/// <summary>
/// Credentials for accessing a UAA service instance
/// </summary>
public class UaaCredentials {
    [JsonPropertyName("clientsecret")]
    [JsonRequired]
    public string? ClientSecret { get; set; }
    [JsonPropertyName("clientid")]
    [JsonRequired]
    public string? ClientId { get; set; }
    /// <summary>
    /// URL to uaa service
    /// </summary>
    /// <value></value>
    [JsonPropertyName("url")]
    [JsonRequired]
    public string? Url { get; set; }
    /// <summary>
    /// Public certificate key to verify tokens
    /// </summary>
    /// <value></value>
    [JsonPropertyName("verificationkey")]
    [JsonRequired]
    public string? VerificationKey { get; set; }
    /// <summary>
    /// Identifier of XSAPPNAME specified in mta.yaml
    /// </summary>
    /// <value></value>
    [JsonPropertyName("xsappname")]
    [JsonRequired]
    public string? XsAppName { get; set; }
    /// <summary>
    /// Zone Identifier of UAA
    /// </summary>
    /// <value></value>
    [JsonPropertyName("zoneid")]
    [JsonRequired]
    public string? ZoneId { get; set; }
    [JsonPropertyName("uaadomain")]
    [JsonRequired]
    public string? UaaDomain { get; set; }
}
