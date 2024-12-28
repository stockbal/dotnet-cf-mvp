// Copyright (c) Demo AG. All Rights Reserved.

using Microsoft.IdentityModel.Tokens;

namespace DevEpos.CF.Demo.Authentication;

/// <summary>
/// Manages access to keys like XSUAA public key
/// </summary>
public interface IKeyManager {
    SecurityKey GetSecurityKey(string kid, SecurityToken token);
}

