using System.Runtime.CompilerServices;

namespace AzureApimPolicyGen;

public interface IPolicyDocument
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    IPolicyDocument AuthenticationBasic(PolicyExpression username, PolicyExpression password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    IPolicyDocument AuthenticationCertificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);

    // TODO: use in 'send-request'
    // https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy#use-managed-identity-in-send-request-policy
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    IPolicyDocument AuthenticationManagedIdentity(PolicyExpression resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = false);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy</summary>
    IPolicyDocument CacheLookup(PolicyExpression varyByDeveloper, PolicyExpression VarByDeveloperGroup,
        PolicyExpression? allowPrivateResponseCaching = null,
        CacheType? cacheType = null,
        PolicyExpression? downstreamCacheType = null, PolicyExpression? mustRevalidate = null,
        Action<ICacheLookupVaryBy>? varyBy = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IPolicyDocument CacheLookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    IPolicyDocument CacheStore(PolicyExpression duration, PolicyExpression? cacheResponse = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IPolicyDocument CacheStoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IPolicyDocument CacheRemoveValue(PolicyExpression key, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/check-header-policy</summary>
    IPolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode, PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyDocument Choose(Action<IChooseActions> choose);

    IPolicyDocument Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    IPolicyDocument FindAndReplace(PolicyExpression from, PolicyExpression to);

    ///https://learn.microsoft.com/en-us/azure/api-management/forward-request-policy
    IPolicyDocument ForwardRequest(HttpVersion? httpVersion = null,
        PolicyExpression? timeoutSeconds = null, PolicyExpression? timeoutMilliseconds = null, PolicyExpression? continueTimeout = null,
        bool? followRedirects = null, bool? bufferRequestBody = null, bool? bufferResponse = null, bool? failOnErrorStatusCode = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);
}

public abstract partial class PolicyDocument : IPolicyDocument
{
    private PolicySection _section = PolicySection.None;
    private PolicyScopes _scopes;
    private PolicyXmlWriter? _writer;
    private PolicyXmlWriter Writer => _writer
        ?? throw new InvalidOperationException("PolicyXmlWriter was not initialized.");

    protected PolicyDocument(PolicyScopes policyScopes = PolicyScopes.All)
        => _scopes = policyScopes;

    protected virtual void Inbound()
    {
        Writer.Base();
    }

    protected virtual void Backend()
    {
        Writer.Base();
    }

    protected virtual void Outbound()
    {
        Writer.Base();
    }

    protected virtual void OnError()
    {
        Writer.Base();
    }

    // ------------------------------------------------------------------------

    protected IPolicyDocument Base()
    {
        Writer.Base();
        return this;
    }

    // ------------------------------------------------------------------------

    internal void WriteTo(Stream stream)
    {
        _writer = new PolicyXmlWriter(stream);

        _section = PolicySection.Inbound;
        _writer.Inbound(Inbound);

        _section = PolicySection.Backend;
        _writer.Backend(Backend);

        _section = PolicySection.Outbound;
        _writer.Outbound(Outbound);

        _section = PolicySection.OnError;
        _writer.OnError(OnError);

        _writer.Close();

        _writer = null;
        _section = PolicySection.None;
    }

    // ------------------------------------------------------------------------

    private void AssertSection(PolicySection expected, [CallerMemberName] string callerName = "")
    {
        if (_section != expected)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in section {_section.ToString()}.");
    }
    private void AssertSection(PolicySection[] expected, [CallerMemberName] string callerName = "")
    {
        if (!expected.Contains(_section))
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in section {_section.ToString()}.");
    }
    private void AssertScopes(PolicyScopes policyScopes, [CallerMemberName] string callerName = "")
    {
        if ((_scopes & policyScopes) == 0)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in scope(s) {_scopes.ToString()}.");
    }
    private static string? CacheTypeToString(CacheType? cacheType)
        => cacheType switch
        {
            CacheType.Internal => "internal",
            CacheType.External => "external",
            CacheType.PreferExternal => "prefer-external",
            _ => null
        };
}

internal enum PolicySection
{
    None,
    Inbound,
    Backend,
    Outbound,
    OnError
}

[Flags]
public enum PolicyScopes
{
    /// <summary>Not set.</summary>
    None = 0x00,
    /// <summary>Policy at global level.</summary>
    Global = 0x01,
    /// <summary>Policy at workspace level.</summary>
    Workspace = 0x02,
    /// <summary>Policy at product level.</summary>
    Product = 0x04,
    /// <summary>Policy at api level.</summary>
    Api = 0x08,
    /// <summary>Policy at api-operation level.</summary>
    Operation = 0x10,
    /// <summary>Policy at any level (except fragments).</summary>
    All = 0x1F,

    /// <summary>Policy fragment used in other policies.</summary>
    Fragment = 0x80,
}
