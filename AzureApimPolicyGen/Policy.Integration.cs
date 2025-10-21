namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#integration-and-external-communication

public interface IIntegration
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyDocument PublishToDapr(PolicyExpression message, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyDocument PublishToDapr(LiquidTemplate template, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null);
}

partial class PolicyDocument
{
    public IPolicyDocument PublishToDapr(PolicyExpression message, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        Writer.PublishToDapr(message, topic, null, pubSubName, responseVariableName, timeout, contentType, ignoreError);
        return this;
    }
    public IPolicyDocument PublishToDapr(LiquidTemplate template, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        Writer.PublishToDapr(template, topic, "Liquid", pubSubName, responseVariableName, timeout, contentType, ignoreError);
        return this;
    }
}

partial class PolicyXmlWriter
{
    public void PublishToDapr(string message, string topic, string? template, string? pubSubName, string? responseVariableName, string? timeout, string? contentType, bool? ignoreError)
    {
        _xmlWriter.WriteStartElement("publish-to-dapr");
        _xmlWriter.WriteAttributeString("topic", topic);
        _xmlWriter.WriteAttributeStringOpt("template", template);
        _xmlWriter.WriteAttributeStringOpt("pubsub-name", pubSubName);
        _xmlWriter.WriteAttributeStringOpt("response-variable-name", responseVariableName);
        _xmlWriter.WriteAttributeStringOpt("content-type", contentType);
        _xmlWriter.WriteAttributeStringOpt("timeout", timeout);
        _xmlWriter.WriteAttributeStringOpt("ignore-error", BoolValue(ignoreError));
        _xmlWriter.WriteString(message);
        _xmlWriter.WriteEndElement();
    }
}
