namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#logging

public interface ILogging
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IPolicyDocument LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression message);
}

public interface IEmitMetricDimensions
{
    IEmitMetricDimensions Add(string name, string? value);
}

partial class PolicyDocument
{
    public IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.EmitMetric(name, @namespace, value, () => dimensions(new EmitMetricDimensions(Writer)));
        return this;
    }

    private sealed class EmitMetricDimensions : IEmitMetricDimensions
    {
        private readonly PolicyXmlWriter _writer;
        internal EmitMetricDimensions(PolicyXmlWriter writer) => _writer = writer;

        private int _count;

        public IEmitMetricDimensions Add(string name, string? value)
        {
            if (_count > 5)
                throw new ArgumentOutOfRangeException("<dimension>", "A maximum of 5 dimensions can be specified for emit-metric.");

            _count++;
            _writer.EmitMetricDimension(name, value);
            return this;
        }
    }

    public IPolicyDocument LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression message)
    {
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        var idEmpty = String.IsNullOrEmpty(partitionId);
        var keyEmpty = String.IsNullOrEmpty(partitionKey);
        if (idEmpty && keyEmpty) throw new ArgumentException($"Either {nameof(partitionId)} or {nameof(partitionKey)} has to be filled.", $"{nameof(partitionId)}+{nameof(partitionKey)}");
        if (!idEmpty && !keyEmpty) throw new ArgumentException($"Either {nameof(partitionId)} or {nameof(partitionKey)} has to be filled. Not both.", $"{nameof(partitionId)}+{nameof(partitionKey)}");
        Writer.LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }
}

partial class PolicyXmlWriter
{
    public void EmitMetric(string name, string? @namespace, string? value, Action writeDimensions)
    {
        _xmlWriter.WriteStartElement("emit-metric");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeStringOpt("value", value);
        _xmlWriter.WriteAttributeStringOpt("namespace", @namespace);
        writeDimensions();
        _xmlWriter.WriteEndElement();
    }
    internal void EmitMetricDimension(string name, string? value)
    {
        _xmlWriter.WriteStartElement("dimension");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeStringOpt("value", value);
        _xmlWriter.WriteEndElement();
    }

    public void LogToEventHub(string loggerId, string? partitionId, string? partitionKey, string message)
    {
        _xmlWriter.WriteStartElement("log-to-eventhub");
        _xmlWriter.WriteAttributeString("logger-id", loggerId);
        _xmlWriter.WriteAttributeStringOpt("partition-id", partitionId);
        _xmlWriter.WriteAttributeStringOpt("partition-key", partitionKey);
        _xmlWriter.WriteString(message);
        _xmlWriter.WriteEndElement();
    }
}