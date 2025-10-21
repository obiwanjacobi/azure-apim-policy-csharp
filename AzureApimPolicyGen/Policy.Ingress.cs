namespace AzureApimPolicyGen;

//https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#rate-limiting-and-quotas
public interface IIngress
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IPolicyDocument LimitConcurrency(PolicyExpression key, int maxCount, Action<IPolicyDocument> actions);
}

partial class PolicyDocument
{
    public IPolicyDocument LimitConcurrency(PolicyExpression key, int maxCount, Action<IPolicyDocument> actions)
    {
        AssertScopes(PolicyScopes.All);
        Writer.LimitConcurrency(key, maxCount.ToString(), () => actions(this));
        return this;
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
}
