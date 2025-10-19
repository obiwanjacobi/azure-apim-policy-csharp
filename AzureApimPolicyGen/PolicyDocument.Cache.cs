namespace AzureApimPolicyGen;


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

partial class PolicyDocument : ICache
{
    public ICache Cache => (ICache)this;

    IPolicyDocument ICache.Lookup(PolicyExpression varyByDeveloper, PolicyExpression varyByDeveloperGroups,
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

    IPolicyDocument ICache.LookupValue(string variableName, PolicyExpression key, PolicyExpression? defaultValue, CacheType? cacheType)
    {
        // TODO: check variable exists
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheLookupValue(variableName, key, defaultValue, CacheTypeToString(cacheType));
        return this;
    }

    IPolicyDocument ICache.Store(PolicyExpression duration, PolicyExpression? cacheResponse)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Writer.CacheStore(duration, cacheResponse);
        return this;
    }

    IPolicyDocument ICache.StoreValue(PolicyExpression duration, PolicyExpression key, PolicyExpression value, CacheType? cacheType)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.CacheStoreValue(duration, key, value, CacheTypeToString(cacheType));
        return this;
    }

    IPolicyDocument ICache.RemoveValue(PolicyExpression key, CacheType? cacheType)
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