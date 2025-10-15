using System.Runtime.CompilerServices;

namespace AzureApimPolicyGen;

public abstract class PolicyDocument :
    IAuthentication,
    ICache
{
    private PolicySection _section = PolicySection.None;
    private PolicyScopes _scopes = PolicyScopes.None;
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

    protected PolicyDocument Base()
    {
        Writer.Base();
        return this;
    }

    public IAuthentication Authentication => (IAuthentication)this;
    public ICache Cache => (ICache)this;

    // ------------------------------------------------------------------------

    internal void WriteTo(Stream stream)
    {
        _writer = new PolicyXmlWriter(stream);

        _section = PolicySection.Inbound;
        Writer.Inbound();
        Inbound();
        Writer.EndElement();

        _section = PolicySection.Backend;
        Writer.Backend();
        Backend();
        Writer.EndElement();

        _section = PolicySection.Outbound;
        Writer.Outbound();
        Outbound();
        Writer.EndElement();

        _section = PolicySection.OnError;
        Writer.OnError();
        OnError();
        Writer.EndElement();

        Writer.Close();
    }

    // ------------------------------------------------------------------------

    PolicyDocument IAuthentication.Basic(string username, string password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.AuthenticationBasic(username, password);
        return this;
    }

    PolicyDocument IAuthentication.Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body, PolicyExpression? password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        if (!String.IsNullOrEmpty(thumbprint) && !String.IsNullOrEmpty(certificate))
            throw new ArgumentException("Specify either a thumbprint or a certificate.  Not both.", $"{nameof(thumbprint)}+{nameof(certificate)}");

        Writer.AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    PolicyDocument IAuthentication.ManagedIdentity(PolicyExpression resource, string? clientId, PolicyVariable? outputTokenVariableName, bool ignoreError)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        // TODO: check variable exists
        Writer.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }

    PolicyDocument ICache.Lookup(PolicyExpression varyByDeveloper, PolicyExpression varyByDeveloperGroups,
        PolicyExpression? allowPrivateResponseCaching, CacheType? cacheType, PolicyExpression? downstreamCacheType,
        PolicyExpression? mustRevalidate, Action<ICacheLookupVaryBy>? varyBy)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? varyByItems = varyBy is null ? null : () => varyBy(new CacheLookupVaryBy(Writer));
        Writer.CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching,
            CacheTypeToString(cacheType), downstreamCacheType, mustRevalidate, varyByItems);
        return this;
    }

    PolicyDocument ICache.LookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue, CacheType? cacheType)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheLookupValue(variableName, key, defaultValue, CacheTypeToString(cacheType));
        return this;
    }

    PolicyDocument ICache.Store(PolicyExpression duration, PolicyExpression? cacheResponse)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Writer.CacheStore(duration, cacheResponse);
        return this;
    }

    PolicyDocument ICache.StoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheStoreValue(duration, key, value, CacheTypeToString(cacheType));
        return this;
    }

    PolicyDocument ICache.RemoveValue(PolicyExpression key, CacheType? cacheType)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheRemoveValue(key, CacheTypeToString(cacheType));
        return this;
    }

    public PolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode,
        PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? writeValues = values is null ? null : () => values(new CheckHeaderValues(Writer));
        Writer.CheckHeader(name, failedCheckHttpCode, failedCheckErrorMessage, ignoreCase, writeValues);
        return this;
    }

    // ------------------------------------------------------------------------

    private sealed class CacheLookupVaryBy : ICacheLookupVaryBy
    {
        private readonly PolicyXmlWriter _writer;
        internal CacheLookupVaryBy(PolicyXmlWriter writer) => _writer = writer;

        public ICacheLookupVaryBy Header(string name)
        {
            _writer.CacheLookup_VaryByHeader(name);
            return this;
        }

        public ICacheLookupVaryBy QueryParam(string name)
        {
            _writer.CacheLookup_VaryByParam(name);
            return this;
        }

        public ICacheLookupVaryBy QueryParams(params string[] names)
        {
            _writer.CacheLookup_VaryByHeader(String.Join(";", names));
            return this;
        }
    }

    private sealed class CheckHeaderValues : ICheckHeaderValues
    {
        private readonly PolicyXmlWriter _writer;
        internal CheckHeaderValues(PolicyXmlWriter writer) => _writer = writer;

        public ICheckHeaderValues Add(string value)
        {
            _writer.CheckHeaderValue(value);
            return this;
        }
    }

    private void AssertSection(PolicySection expected, [CallerMemberName] string callerName = "")
    {
        if (_section != expected)
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
