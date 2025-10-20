namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#caching

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

partial class PolicyDocument
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy</summary>
    public IPolicyDocument CacheLookup(PolicyExpression varyByDeveloper, PolicyExpression varyByDeveloperGroups,
        PolicyExpression? allowPrivateResponseCaching = null, CacheType? cacheType = null, PolicyExpression? downstreamCacheType = null,
        PolicyExpression? mustRevalidate = null, Action<ICacheLookupVaryBy>? varyBy = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? varyByItems = varyBy is null ? null : () => varyBy(new CacheLookupVaryBy(Writer));
        Writer.CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching,
            CacheTypeToString(cacheType), downstreamCacheType, mustRevalidate, varyByItems);
        return this;
    }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    public IPolicyDocument CacheLookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue = null, CacheType? cacheType = null)
    {
        // TODO: check variable exists
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheLookupValue(variableName, key, defaultValue, CacheTypeToString(cacheType));
        return this;
    }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    public IPolicyDocument CacheStore(PolicyExpression duration, PolicyExpression? cacheResponse = null)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Writer.CacheStore(duration, cacheResponse);
        return this;
    }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    public IPolicyDocument CacheStoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType = null)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheStoreValue(duration, key, value, CacheTypeToString(cacheType));
        return this;
    }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    public IPolicyDocument CacheRemoveValue(PolicyExpression key, CacheType? cacheType = null)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
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