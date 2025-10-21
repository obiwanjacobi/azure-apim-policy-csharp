namespace AzureApimPolicyGen;

//https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#rate-limiting-and-quotas

public interface IIngress
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IPolicyDocument LimitConcurrency(PolicyExpression key, int maxCount, Action<IPolicyDocument> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/quota-policy</summary>
    IPolicyDocument Quota(int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApi> apis);
}

public interface IQuotaApi
{
    IQuotaApi Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApiOperation> operations);
}

public interface IQuotaApiOperation
{
    IQuotaApiOperation Add(string? id, string? name, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds);
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

        private static void ValidateNameId(string? id, string? name)
        {
            var idEmpty = String.IsNullOrEmpty(id);
            var nameEmpty = String.IsNullOrEmpty(name);
            if (idEmpty && nameEmpty)
                throw new ArgumentException($"Either {nameof(id)} and {nameof(name)} must be filled.", $"{nameof(id)}+{nameof(name)}");
            if (!idEmpty && !nameEmpty)
                throw new ArgumentException($"Either {nameof(id)} and {nameof(name)} must be filled. Not Both", $"{nameof(id)}+{nameof(name)}");
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
}
