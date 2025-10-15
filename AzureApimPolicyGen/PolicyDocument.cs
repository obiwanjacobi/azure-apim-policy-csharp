using System.Runtime.CompilerServices;

namespace AzureApimPolicyGen;

public abstract class PolicyDocument :
    IAuthentication,
    ICache
{
    private PolicyWriterPhase _phase = PolicyWriterPhase.None;
    private PolicyXmlWriter? _writer;
    private PolicyXmlWriter Writer => _writer
        ?? throw new InvalidOperationException("PolicyXmlWriter was not initialized.");

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

    protected IAuthentication Authentication => (IAuthentication)this;
    protected ICache Cache => (ICache)this;


    // ------------------------------------------------------------------------

    internal void WriteTo(string path)
    {
        _writer = new PolicyXmlWriter(path);

        _phase = PolicyWriterPhase.Inbound;
        Writer.Inbound();
        Inbound();
        Writer.EndElement();

        _phase = PolicyWriterPhase.Backend;
        Writer.Backend();
        Backend();
        Writer.EndElement();

        _phase = PolicyWriterPhase.Outbound;
        Writer.Outbound();
        Outbound();
        Writer.EndElement();

        _phase = PolicyWriterPhase.OnError;
        Writer.OnError();
        OnError();
        Writer.EndElement();

        Writer.Close();
    }

    // ------------------------------------------------------------------------

    PolicyDocument IAuthentication.Basic(string username, string password)
    {
        AssertPhase(PolicyWriterPhase.Inbound);
        Writer.AuthenticationBasic(username, password);
        return this;
    }

    PolicyDocument IAuthentication.Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body, PolicyExpression? password)
    {
        AssertPhase(PolicyWriterPhase.Inbound);
        if (!String.IsNullOrEmpty(thumbprint) && !String.IsNullOrEmpty(certificate))
            throw new ArgumentException("Specify either a thumbprint or a certificate.", $"{nameof(thumbprint)}+{nameof(certificate)}");

        Writer.AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    PolicyDocument IAuthentication.ManagedIdentity(PolicyExpression resource, string? clientId, PolicyVariable? outputTokenVariableName, bool ignoreError)
    {
        AssertPhase(PolicyWriterPhase.Inbound);
        // TODO: check variable exists
        Writer.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }

    PolicyDocument ICache.Lookup(PolicyExpression varyByDeveloper, PolicyExpression varyByDeveloperGroups,
        PolicyExpression? allowPrivateResponseCaching, CacheType? cacheType, PolicyExpression? downstreamCacheType,
        PolicyExpression? mustRevalidate, Action<ICacheLookupVaryBy>? varyBy)
    {
        AssertPhase(PolicyWriterPhase.Inbound);
        Action? varyByItems = varyBy is null ? null : VaryByItems;
        Writer.CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching,
            CacheTypeToString(cacheType), downstreamCacheType, mustRevalidate, varyByItems);
        return this;

        void VaryByItems()
        {
            var varyByCtx = new CacheLookupVaryBy(_writer);
            varyBy(varyByCtx);
        }

        static string? CacheTypeToString(CacheType? cacheType)
            => cacheType switch
            {
                CacheType.Internal => "internal",
                CacheType.External => "external",
                CacheType.PreferExternal => "prefer-external",
                _ => null
            };
    }

    private class CacheLookupVaryBy : ICacheLookupVaryBy
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

    private void AssertPhase(PolicyWriterPhase expected, [CallerMemberName] string callerName = "")
    {
        if (_phase != expected)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in {_phase.ToString()}.");
    }
}

internal enum PolicyWriterPhase
{
    None,
    Inbound,
    Backend,
    Outbound,
    OnError
}
