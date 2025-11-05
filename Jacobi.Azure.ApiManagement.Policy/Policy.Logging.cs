namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#logging

internal interface ILogging
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IPolicyDocument LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/trace-policy</summary>
    IPolicyDocument Trace(string source, PolicyExpression<string> message, TraceSeverity severity = TraceSeverity.Verbose, string? metadataName = null, string? metadataValue = null);
}

public interface IEmitMetricDimensions
{
    IEmitMetricDimensions Add(string name, string? value);
}

public enum TraceSeverity
{
    Verbose,
    Information,
    Error
}

partial class PolicyDocumentBase
{
    internal PolicyDocumentBase EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
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

    internal PolicyDocumentBase LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        var idEmpty = String.IsNullOrEmpty(partitionId);
        var keyEmpty = String.IsNullOrEmpty(partitionKey);
        if (idEmpty && keyEmpty) throw new ArgumentException($"Either {nameof(partitionId)} or {nameof(partitionKey)} has to be filled.", $"{nameof(partitionId)}+{nameof(partitionKey)}");
        if (!idEmpty && !keyEmpty) throw new ArgumentException($"Either {nameof(partitionId)} or {nameof(partitionKey)} has to be filled. Not both.", $"{nameof(partitionId)}+{nameof(partitionKey)}");
        Writer.LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }

    internal PolicyDocumentBase Trace(string source, PolicyExpression<string> message, TraceSeverity severity = TraceSeverity.Verbose, string? metadataName = null, string? metadataValue = null)
    {
        if (((metadataName is null && metadataValue is not null) ||
             (metadataName is not null && metadataValue is null)))
            throw new ArgumentException($"Both {nameof(metadataName)} and {metadataValue} must be filled or null.", $"{nameof(metadataName)}+{metadataValue}");

        Writer.Trace(source, message, SeverityToString(severity), metadataName, metadataValue);
        return this;

        static string SeverityToString(TraceSeverity severity)
            => severity switch
            {
                TraceSeverity.Verbose => "verbose",
                TraceSeverity.Information => "information",
                TraceSeverity.Error => "error",
                _ => "verbose"
            };
    }
}

partial class PolicyDocument
{
    IInbound IInbound.EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        EmitMetric(name, @namespace, value, dimensions);
        return this;
    }
    IBackend IBackend.EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        EmitMetric(name, @namespace, value, dimensions);
        return this;
    }
    IOutbound IOutbound.EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        EmitMetric(name, @namespace, value, dimensions);
        return this;
    }
    IOnError IOnError.EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        EmitMetric(name, @namespace, value, dimensions);
        return this;
    }

    IInbound IInbound.LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }
    IBackend IBackend.LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }
    IOutbound IOutbound.LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }
    IOnError IOnError.LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }

    IInbound IInbound.Trace(string source, PolicyExpression<string> message, TraceSeverity severity, string? metadataName, string? metadataValue)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Trace(source, message, severity, metadataName, metadataValue);
        return this;
    }
    IBackend IBackend.Trace(string source, PolicyExpression<string> message, TraceSeverity severity, string? metadataName, string? metadataValue)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        Trace(source, message, severity, metadataName, metadataValue);
        return this;
    }
    IOutbound IOutbound.Trace(string source, PolicyExpression<string> message, TraceSeverity severity, string? metadataName, string? metadataValue)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Trace(source, message, severity, metadataName, metadataValue);
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

    public void Trace(string source, string message, string? severity, string? metaName, string? metaValue)
    {
        _xmlWriter.WriteStartElement("trace");
        _xmlWriter.WriteAttributeString("source", source);
        _xmlWriter.WriteAttributeStringOpt("severity", severity);
        _xmlWriter.WriteStartElement("message");
        _xmlWriter.WriteString(message);
        _xmlWriter.WriteEndElement();
        if (metaName is not null && metaValue is not null)
        {
            _xmlWriter.WriteStartElement("metadata");
            _xmlWriter.WriteAttributeString("name", metaName);
            _xmlWriter.WriteAttributeString("value", metaValue);
            _xmlWriter.WriteEndElement();
        }
        _xmlWriter.WriteEndElement();
    }
}