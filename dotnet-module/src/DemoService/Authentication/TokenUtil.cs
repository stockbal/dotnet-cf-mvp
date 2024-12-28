// Copyright (c) Demo AG. All Rights Reserved.

using System.IdentityModel.Tokens.Jwt;

namespace DevEpos.CF.Demo.Authenticatio;

public class TokenUtil {
    /// <summary>
    /// Retrieves User id (email or client_id claim) from authorization header
    /// </summary>
    /// <param name="requestHeaders">request headers from http request</param>
    /// <returns>the found user id or <code>null</code></returns>
    public static string? GetUserIdClaimFromAuthHeader(IHeaderDictionary requestHeaders) {
        var authHeader = requestHeaders["authorization"];
        if (authHeader.Count == 1) {
            var token = new JwtSecurityToken(authHeader[0]!.Substring(7));
            var userMail = token.Claims.Where(c => c.Type == "email").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(userMail)) {
                return userMail;
            }
            var clientId = token.Claims.Where(c => c.Type == "client_id").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(clientId)) {
                return clientId;
            }
        }
        return null;
    }
}
