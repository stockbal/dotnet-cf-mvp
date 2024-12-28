// Copyright (c) Demo AG. All Rights Reserved.

namespace DevEpos.CF.Demo.Env;

/// <summary>
/// Provides access to environment variables of services (e.g. XSUAA, SDM, ....)
/// </summary>
public interface IServiceEnv {
    /// <summary>
    /// Access Credentials of bound UAA instance
    /// </summary>
    public List<UaaCredentials> XsuaaCredentials { get; }
}
