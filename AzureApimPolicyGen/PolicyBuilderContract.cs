namespace AzureApimPolicyGen;

public interface IAuthentication
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    PolicyDocument Basic(string username, string password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    PolicyDocument Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);

    // TODO: use in 'send-request'
    // https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy#use-managed-identity-in-send-request-policy
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

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    PolicyDocument LookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    PolicyDocument Store(PolicyExpression duration, PolicyExpression? cacheResponse = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    PolicyDocument StoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    PolicyDocument RemoveValue(PolicyExpression key, CacheType? cacheType = null);
}

public interface ICacheLookupVaryBy
{
    ICacheLookupVaryBy Header(string name);
    ICacheLookupVaryBy QueryParam(string name);
    ICacheLookupVaryBy QueryParams(params string[] names);
}

public interface ICheckHeaderValues
{
    ICheckHeaderValues Add(string value);
}