namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#caching

public interface ICaching
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy</summary>
    IPolicyFragment CacheLookup(PolicyExpression<bool> varyByDeveloper, PolicyExpression<bool> varyByDeveloperGroups, PolicyExpression<bool>? allowPrivateResponseCaching = null, CacheType? cacheType = null, PolicyExpression<string>? downstreamCacheType = null, PolicyExpression<bool>? mustRevalidate = null, Action<ICacheLookupVaryBy>? varyBy = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IPolicyFragment CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    IPolicyFragment CacheStore(PolicyExpression<int> durationSeconds, PolicyExpression<bool>? cacheResponse = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IPolicyFragment CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IPolicyFragment CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null);
}

public enum CacheType
{
    PreferExternal,
    Internal,
    External,
}

public interface ICacheLookupVaryBy
{
    ICacheLookupVaryBy Header(string name);
    ICacheLookupVaryBy QueryParam(string name);
    ICacheLookupVaryBy QueryParams(params string[] names);
}

partial class PolicyDocumentBase
{
    internal PolicyDocumentBase CacheLookup(PolicyExpression<bool> varyByDeveloper, PolicyExpression<bool> varyByDeveloperGroups, PolicyExpression<bool>? allowPrivateResponseCaching, CacheType? cacheType, PolicyExpression<string>? downstreamCacheType, PolicyExpression<bool>? mustRevalidate, Action<ICacheLookupVaryBy>? varyBy)
    {
        Action? varyByItems = varyBy is null ? null : () => varyBy(new CacheLookupVaryBy(Writer));
        Writer.CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching,
            CacheTypeToString(cacheType), downstreamCacheType, mustRevalidate, varyByItems);
        return this;
    }

    internal PolicyDocumentBase CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null)
    {
        Writer.CacheLookupValue(variableName, key, defaultValue, CacheTypeToString(cacheType));
        return this;
    }

    internal PolicyDocumentBase CacheStore(PolicyExpression<int> durationSeconds, PolicyExpression<bool>? cacheResponse)
    {
        Writer.CacheStore(durationSeconds, cacheResponse);
        return this;
    }

    internal PolicyDocumentBase CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null)
    {
        Writer.CacheStoreValue(durationSeconds, key, value, CacheTypeToString(cacheType));
        return this;
    }

    internal PolicyDocumentBase CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null)
    {
        Writer.CacheRemoveValue(key, CacheTypeToString(cacheType));
        return this;
    }

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

    private static string? CacheTypeToString(CacheType? cacheType)
        => cacheType switch
        {
            CacheType.Internal => "internal",
            CacheType.External => "external",
            CacheType.PreferExternal => "prefer-external",
            _ => null
        };
}

partial class PolicyDocument
{
    IInbound IInbound.CacheLookup(PolicyExpression<bool> varyByDeveloper, PolicyExpression<bool> varyByDeveloperGroups, PolicyExpression<bool>? allowPrivateResponseCaching, CacheType? cacheType, PolicyExpression<string>? downstreamCacheType, PolicyExpression<bool>? mustRevalidate, Action<ICacheLookupVaryBy>? varyBy)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching, cacheType, downstreamCacheType, mustRevalidate, varyBy);
        return this;
    }

    IInbound IInbound.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        CacheLookupValue(variableName, key, defaultValue, cacheType);
        return this;
    }
    IBackend IBackend.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        CacheLookupValue(variableName, key, defaultValue, cacheType);
        return this;
    }
    IOutbound IOutbound.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        CacheLookupValue(variableName, key, defaultValue, cacheType);
        return this;
    }
    IOnError IOnError.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        CacheLookupValue(variableName, key, defaultValue, cacheType);
        return this;
    }

    IOutbound IOutbound.CacheStore(PolicyExpression<int> durationSeconds, PolicyExpression<bool>? cacheResponse)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        CacheStore(durationSeconds, cacheResponse);
        return this;
    }

    IInbound IInbound.CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        CacheStoreValue(durationSeconds, key, value, cacheType);
        return this;
    }
    IBackend IBackend.CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        CacheStoreValue(durationSeconds, key, value, cacheType);
        return this;
    }
    IOutbound IOutbound.CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        CacheStoreValue(durationSeconds, key, value, cacheType);
        return this;
    }
    IOnError IOnError.CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        CacheStoreValue(durationSeconds, key, value, cacheType);
        return this;
    }

    IInbound IInbound.CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        CacheRemoveValue(key, cacheType);
        return this;
    }
    IBackend IBackend.CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        CacheRemoveValue(key, cacheType);
        return this;
    }
    IOutbound IOutbound.CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        CacheRemoveValue(key, cacheType);
        return this;
    }
    IOnError IOnError.CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        CacheRemoveValue(key, cacheType);
        return this;
    }
}

partial class PolicyXmlWriter
{
    public void CacheLookup(string varyByDeveloper, string varyByDeveloperGroups, string? allowPrivateResponseCaching,
        string? cacheType, string? downstreamCacheType, string? mustRevalidate, Action? varyBy)
    {
        _xmlWriter.WriteStartElement("cache-lookup");
        _xmlWriter.WriteAttributeString("vary-by-developer", varyByDeveloper);
        _xmlWriter.WriteAttributeString("vary-by-developer-groups", varyByDeveloperGroups);
        _xmlWriter.WriteAttributeStringOpt("allow-private-response-caching", allowPrivateResponseCaching);
        _xmlWriter.WriteAttributeStringOpt("caching-type", cacheType);
        _xmlWriter.WriteAttributeStringOpt("downstream-caching-type", downstreamCacheType);
        _xmlWriter.WriteAttributeStringOpt("must-revalidate", mustRevalidate);
        if (varyBy is not null) varyBy();
        _xmlWriter.WriteEndElement();
    }
    public void CacheLookup_VaryByHeader(string name)
        => _xmlWriter.WriteElementString("vary-by-header", name);
    public void CacheLookup_VaryByParam(string nameOrNames)
        => _xmlWriter.WriteElementString("vary-by-query-parameter", nameOrNames);

    public void CacheLookupValue(string variableName, string key, string? defaultValue, string? cacheType)
    {
        _xmlWriter.WriteStartElement("cache-lookup-value");
        _xmlWriter.WriteAttributeString("variable-name", variableName);
        _xmlWriter.WriteAttributeString("key", key);
        _xmlWriter.WriteAttributeStringOpt("default-value", defaultValue);
        _xmlWriter.WriteAttributeStringOpt("caching-type", cacheType);
        _xmlWriter.WriteEndElement();
    }

    public void CacheStore(string duration, string? cacheResponse)
    {
        _xmlWriter.WriteStartElement("cache-store");
        _xmlWriter.WriteAttributeString("duration", duration);
        _xmlWriter.WriteAttributeStringOpt("cache-response", cacheResponse);
        _xmlWriter.WriteEndElement();
    }

    public void CacheStoreValue(string duration, string key, string value, string? cacheType)
    {
        _xmlWriter.WriteStartElement("cache-store-value");
        _xmlWriter.WriteAttributeString("duration", duration);
        _xmlWriter.WriteAttributeString("key", key);
        _xmlWriter.WriteAttributeString("value", value);
        _xmlWriter.WriteAttributeStringOpt("cache-type", cacheType);
        _xmlWriter.WriteEndElement();
    }

    public void CacheRemoveValue(string key, string? cacheType)
    {
        _xmlWriter.WriteStartElement("cache-remove-value");
        _xmlWriter.WriteAttributeString("key", key);
        _xmlWriter.WriteAttributeStringOpt("cache-type", cacheType);
        _xmlWriter.WriteEndElement();
    }
}