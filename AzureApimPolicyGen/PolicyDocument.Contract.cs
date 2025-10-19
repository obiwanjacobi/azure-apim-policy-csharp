namespace AzureApimPolicyGen;

public interface IPolicyDocument
{
    IAuthentication Authentication { get; }
    ICache Cache { get; }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/check-header-policy</summary>
    IPolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode, PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyDocument Choose(Action<IChooseActions> choose);

    IPolicyDocument Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);
}

public interface IAuthentication
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    IPolicyDocument Basic(string username, string password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    IPolicyDocument Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);

    // TODO: use in 'send-request'
    // https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy#use-managed-identity-in-send-request-policy
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    IPolicyDocument ManagedIdentity(PolicyExpression resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool ignoreError = false);
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
    IPolicyDocument Lookup(PolicyExpression varyByDeveloper, PolicyExpression VarByDeveloperGroup,
        PolicyExpression? allowPrivateResponseCaching = null,
        CacheType? cacheType = null,
        PolicyExpression? downstreamCacheType = null, PolicyExpression? mustRevalidate = null,
        Action<ICacheLookupVaryBy>? varyBy = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IPolicyDocument LookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    IPolicyDocument Store(PolicyExpression duration, PolicyExpression? cacheResponse = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IPolicyDocument StoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IPolicyDocument RemoveValue(PolicyExpression key, CacheType? cacheType = null);
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

public interface IChooseActions
{
    IChooseActions When(PolicyExpression condition, Action<IPolicyDocument> whenActions);
    IChooseActions Otherwise(Action<IPolicyDocument> otherwiseActions);
}

public interface ICorsActions
{
    ICorsActions AllowedOrigins(Action<ICorsAllowedOrigins> origins);
    ICorsActions AllowedMethods(Action<ICorsAllowedMethods> methods, int? preFlightResultMaxAge = null);
    ICorsActions AllowedHeaders(Action<ICorsAllowedHeaders> headers);
    ICorsActions ExposeHeaders(Action<ICorsExposedHeaders> headers);
}

public interface ICorsAllowedOrigins
{
    ICorsAllowedOrigins Any();
    // TODO: url templates?
    ICorsAllowedOrigins Add(string origin);
}

public interface ICorsAllowedMethods
{
    ICorsAllowedMethods Any();
    ICorsAllowedMethods Add(HttpMethod method);
}

public interface ICorsAllowedHeaders
{
    ICorsAllowedHeaders Add(string header);
}

public interface ICorsExposedHeaders
{
    ICorsExposedHeaders Add(string header);
}
