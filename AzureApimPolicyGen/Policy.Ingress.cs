using System.Globalization;

namespace AzureApimPolicyGen;

//https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#rate-limiting-and-quotas

public interface IIngress
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IPolicyDocument LimitConcurrency(PolicyExpression key, int maxCount, Action<IPolicyDocument> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/quota-policy</summary>
    IPolicyDocument Quota(int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApi> apis);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/quota-by-key-policy</summary>
    IPolicyDocument QuotaByKey(PolicyExpression counterKey, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds,
        PolicyExpression? incrementCount = null, PolicyExpression? incrementCondition = null, DateTime? firstPeriodStart = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rate-limit-policy</summary>
    IPolicyDocument RateLimit(int numberOfCalls, int renewalPeriodSeconds, PolicyVariable? retryAfterVariableName = null, string? retryAfterHeaderName = null,
        PolicyVariable? remainingCallsVariableName = null, string? remainingCallsHeaderName = null, string? totalCallsHeaderName = null,
        Action<IRateLimitApi>? apis = null);
}

public interface IQuotaApi
{
    IQuotaApi Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApiOperation> operations);
}

public interface IQuotaApiOperation
{
    IQuotaApiOperation Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds);
}

public interface IRateLimitApi
{
    IRateLimitApi Add(string? id, string? name, int numberOfCalls, int renewalPeriodSeconds, Action<IRateLimitApiOperation>? operations = null);
}

public interface IRateLimitApiOperation
{
    IRateLimitApiOperation Add(string? id, string? name, int numberOfCalls, int renewalPeriodSeconds);
}

partial class PolicyDocument
{
    public IPolicyDocument LimitConcurrency(PolicyExpression key, int maxCount, Action<IPolicyDocument> actions)
    {
        AssertScopes(PolicyScopes.All);
        Writer.LimitConcurrency(key, maxCount.ToString(), () => actions(this));
        return this;
    }

    public IPolicyDocument Quota(int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApi> apis)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Product);
        QuotaValidate(numberOfCalls, bandwidthKB);
        Writer.Quota(QuotaIntToString(numberOfCalls), QuotaIntToString(bandwidthKB), renewalPeriodSeconds.ToString(), () => apis(new QuotaApi(Writer)));
        return this;
    }

    private static string? QuotaIntToString(int zeroIsNull)
        => zeroIsNull == 0 ? null : zeroIsNull.ToString();

    private static void QuotaValidate(int numberOfCalls, int bandwidthKB)
    {
        if (numberOfCalls == 0 && bandwidthKB == 0)
            throw new ArgumentException($"Both {nameof(numberOfCalls)} and {nameof(bandwidthKB)} cannot be zero.", $"{nameof(numberOfCalls)}+{nameof(bandwidthKB)}");
    }
    private static void ValidateNameId(string? id, string? name)
    {
        var idEmpty = String.IsNullOrEmpty(id);
        var nameEmpty = String.IsNullOrEmpty(name);
        if (idEmpty && nameEmpty)
            throw new ArgumentException($"Either {nameof(id)} and {nameof(name)} must be filled.", $"{nameof(id)}+{nameof(name)}");
        if (!idEmpty && !nameEmpty)
            throw new ArgumentException($"Either {nameof(id)} and {nameof(name)} must be filled. Not Both", $"{nameof(id)}+{nameof(name)}");
    }

    private sealed class QuotaApi : IQuotaApi, IQuotaApiOperation
    {
        private readonly PolicyXmlWriter _writer;
        internal QuotaApi(PolicyXmlWriter writer) { _writer = writer; }

        public IQuotaApi Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApiOperation> operations)
        {
            ValidateNameId(id, name);
            QuotaValidate(numberOfCalls, bandwidthKB);
            _writer.QuotaApi(id, name, QuotaIntToString(numberOfCalls), QuotaIntToString(bandwidthKB),
                renewalPeriodSeconds.ToString(), () => operations(this));
            return this;
        }

        public IQuotaApiOperation Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds)
        {
            ValidateNameId(id, name);
            QuotaValidate(numberOfCalls, bandwidthKB);
            _writer.QuotaApiOperation(id, name, QuotaIntToString(numberOfCalls), QuotaIntToString(bandwidthKB),
                renewalPeriodSeconds.ToString());
            return this;
        }
    }

    public IPolicyDocument QuotaByKey(PolicyExpression counterKey, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds,
        PolicyExpression? incrementCount = null, PolicyExpression? incrementCondition = null, DateTime? firstPeriodStart = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        QuotaValidate(numberOfCalls, bandwidthKB);
        if (renewalPeriodSeconds < 300 && renewalPeriodSeconds != 0)
            throw new ArgumentOutOfRangeException(nameof(renewalPeriodSeconds), "Value must be 300 minimum or 0.");
        Writer.QuotaByKey(counterKey, QuotaIntToString(numberOfCalls), QuotaIntToString(bandwidthKB), renewalPeriodSeconds.ToString(),
            incrementCount, incrementCondition, firstPeriodStart?.ToString("o", CultureInfo.InvariantCulture));
        return this;
    }

    public IPolicyDocument RateLimit(int numberOfCalls, int renewalPeriodSeconds, PolicyVariable? retryAfterVariableName = null, string? retryAfterHeaderName = null,
        PolicyVariable? remainingCallsVariableName = null, string? remainingCallsHeaderName = null, string? totalCallsHeaderName = null,
        Action<IRateLimitApi>? apis = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        if (renewalPeriodSeconds > 300)
            throw new ArgumentOutOfRangeException(nameof(renewalPeriodSeconds), "Value cannot exceed 300.");
        Writer.RateLimit(numberOfCalls.ToString(), renewalPeriodSeconds.ToString(),
            retryAfterVariableName, retryAfterHeaderName, remainingCallsVariableName,
            remainingCallsHeaderName, totalCallsHeaderName,
            apis is null ? null : () => apis(new RateLimitApi(Writer)));
        return this;
    }

    private sealed class RateLimitApi : IRateLimitApi, IRateLimitApiOperation
    {
        private readonly PolicyXmlWriter _writer;
        internal RateLimitApi(PolicyXmlWriter writer) { _writer = writer; }

        public IRateLimitApi Add(string? id, string? name, int numberOfCalls, int renewalPeriodSeconds, Action<IRateLimitApiOperation>? operations = null)
        {
            ValidateNameId(id, name);
            _writer.RateLimitApi(id, name, numberOfCalls.ToString(), renewalPeriodSeconds.ToString(),
                operations is null ? null : () => operations(this));
            return this;
        }

        public IRateLimitApiOperation Add(string? id, string? name, int numberOfCalls, int renewalPeriodSeconds)
        {
            ValidateNameId(id, name);
            _writer.RateLimitApiOperation(id, name, numberOfCalls.ToString(), renewalPeriodSeconds.ToString());
            return this;
        }
    }
}

partial class PolicyXmlWriter
{
    public void LimitConcurrency(string key, string maxCount, Action writeActions)
    {
        _xmlWriter.WriteStartElement("limit-concurrency");
        _xmlWriter.WriteAttributeString("key", key);
        _xmlWriter.WriteAttributeString("max-count", maxCount);
        writeActions();
        _xmlWriter.WriteEndElement();
    }

    public void Quota(string? calls, string? bandwidth, string renewalPeriod, Action writeApis)
    {
        _xmlWriter.WriteStartElement("quota");
        QuotaAttributes(calls, bandwidth, renewalPeriod);
        writeApis();
        _xmlWriter.WriteEndElement();
    }
    internal void QuotaApi(string? id, string? name, string? calls, string? bandwidth, string renewalPeriod, Action writeOperations)
    {
        _xmlWriter.WriteStartElement("api");
        QuotaNameId(id, name);
        QuotaAttributes(calls, bandwidth, renewalPeriod);
        writeOperations();
        _xmlWriter.WriteEndElement();
    }
    internal void QuotaApiOperation(string? id, string? name, string? calls, string? bandwidth, string renewalPeriod)
    {
        _xmlWriter.WriteStartElement("operation");
        QuotaNameId(id, name);
        QuotaAttributes(calls, bandwidth, renewalPeriod);
        _xmlWriter.WriteEndElement();
    }
    internal void QuotaNameId(string? id, string? name)
    {
        _xmlWriter.WriteAttributeStringOpt("id", id);
        _xmlWriter.WriteAttributeStringOpt("name", name);
    }
    internal void QuotaAttributes(string? calls, string? bandwidth, string renewalPeriod)
    {
        _xmlWriter.WriteAttributeStringOpt("calls", calls);
        _xmlWriter.WriteAttributeStringOpt("bandwidth", bandwidth);
        _xmlWriter.WriteAttributeString("renewal-period", renewalPeriod);
    }

    public void QuotaByKey(string counterKey, string? calls, string? bandwidth, string renewalPeriod,
        string? incrementCount, string? incrementCondition, string? firstPeriodStart)
    {
        _xmlWriter.WriteStartElement("quota-by-key");
        _xmlWriter.WriteAttributeString("counter-key", counterKey);
        QuotaAttributes(calls, bandwidth, renewalPeriod);
        _xmlWriter.WriteAttributeStringOpt("increment-count", incrementCount);
        _xmlWriter.WriteAttributeStringOpt("increment-condition", incrementCondition);
        _xmlWriter.WriteAttributeStringOpt("first-period-start", firstPeriodStart);
        _xmlWriter.WriteEndElement();
    }

    public void RateLimit(string calls, string renewalPeriod, string? retryAfterVariableName, string? retryAfterHeaderName,
        string? remainingCallsVariableName, string? remainingCallsHeaderName, string? totalCallsHeaderName, Action? writeApis)
    {
        _xmlWriter.WriteStartElement("rate-limit");
        RateLimitAttributes(calls, renewalPeriod);
        _xmlWriter.WriteAttributeStringOpt("retry-after-variable-name", retryAfterVariableName);
        _xmlWriter.WriteAttributeStringOpt("retry-after-header-name", retryAfterHeaderName);
        _xmlWriter.WriteAttributeStringOpt("remaining-calls-variable-name", remainingCallsVariableName);
        _xmlWriter.WriteAttributeStringOpt("remaining-calls-header-name", remainingCallsHeaderName);
        _xmlWriter.WriteAttributeStringOpt("total-calls-header-name", totalCallsHeaderName);
        if (writeApis is not null) writeApis();
        _xmlWriter.WriteEndElement();
    }
    internal void RateLimitApi(string? id, string? name, string calls, string renewalPeriod, Action? operations)
    {
        _xmlWriter.WriteStartElement("api");
        _xmlWriter.WriteAttributeStringOpt("id", id);
        _xmlWriter.WriteAttributeStringOpt("name", name);
        RateLimitAttributes(calls, renewalPeriod);
        if (operations is not null) operations();
        _xmlWriter.WriteEndElement();
    }
    internal void RateLimitApiOperation(string? id, string? name, string calls, string renewalPeriod)
    {
        _xmlWriter.WriteStartElement("operation");
        _xmlWriter.WriteAttributeStringOpt("id", id);
        _xmlWriter.WriteAttributeStringOpt("name", name);
        RateLimitAttributes(calls, renewalPeriod);
        _xmlWriter.WriteEndElement();
    }
    internal void RateLimitAttributes(string calls, string renewalPeriod)
    {
        _xmlWriter.WriteAttributeString("calls", calls);
        _xmlWriter.WriteAttributeString("renewal-period", renewalPeriod);
    }
}
