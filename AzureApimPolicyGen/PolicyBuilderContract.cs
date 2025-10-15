namespace AzureApimPolicyGen;

public interface IAuthentication
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    PolicyDocument Basic(string username, string password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    PolicyDocument Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    PolicyDocument ManagedIdentity(PolicyExpression resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool ignoreError = false);
}

public enum CacheType
{
    PreferExternal,
    Internal,
    External,
}

public interface ICache
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy</summary>
    PolicyDocument Lookup(PolicyExpression varyByDeveloper, PolicyExpression VarByDeveloperGroup,
        PolicyExpression? allowPrivateResponseCaching = null,
        CacheType? cacheType = null,
        PolicyExpression? downstreamCacheType = null, PolicyExpression? mustRevalidate = null,
        Action<ICacheLookupVaryBy>? varyBy = null);
}

public interface ICacheLookupVaryBy
{
    ICacheLookupVaryBy Header(string name);
    ICacheLookupVaryBy QueryParam(string name);
    ICacheLookupVaryBy QueryParams(params string[] names);
}

