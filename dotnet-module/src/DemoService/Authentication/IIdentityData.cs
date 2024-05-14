namespace DevEpos.CF.Demo.Authentication {
    public interface IIdentityData {
        /// <summary>
        /// Default policy to check general claims
        /// </summary>
        public const string DefaultAuthPolicyName = "Default";
        /// <summary>
        /// Policy to verify user token contains the 'User' Scope
        /// </summary>
        public const string UserPolicyName = "User";
        /// <summary>
        /// User scope name (see xs-security.json) 
        /// </summary>
        public const string UserScopeValue = "User";
    }
}
