namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#integration-and-external-communication

public interface IIntegration
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyDocument PublishToDapr(PolicyExpression message, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyDocument PublishToDapr(LiquidTemplate template, PolicyExpression topic, PolicyExpression? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression? timeout = null, string? contentType = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IPolicyDocument SendOneWayRequest(Action<ISendOneWayRequestActions> request, PolicyExpression? mode = null, PolicyExpression? timeoutSeconds = null);
}

public interface ISendOneWayRequestActions
{
    ISendOneWayRequestActions SetUrl(PolicyExpression url);
    ISendOneWayRequestActions SetMethod(PolicyExpression method);
    ISendOneWayRequestActions SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null);
    ISendOneWayRequestActions SetBody(PolicyExpression body);
    ISendOneWayRequestActions AuthenticationCertificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);
    ISendOneWayRequestActions Proxy(PolicyExpression url, PolicyExpression? username = null, PolicyExpression? password = null);
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

    public IPolicyDocument SendOneWayRequest(Action<ISendOneWayRequestActions> request, PolicyExpression? mode = null, PolicyExpression? timeoutSeconds = null)
    {
        Writer.SendOneWayRequest(mode, timeoutSeconds, () => request(new SendOneWayRequestActions(this, Writer)));
        return this;
    }

    private sealed class SendOneWayRequestActions : ISendOneWayRequestActions
    {
        private readonly PolicyXmlWriter _writer;
        private readonly PolicyDocument _document;
        public SendOneWayRequestActions(PolicyDocument document, PolicyXmlWriter writer)
        {
            _writer = writer;
            _document = document;
        }

        public ISendOneWayRequestActions SetUrl(PolicyExpression url)
        {
            _writer.SendOneWayRequestUrl(url);
            return this;
        }

        public ISendOneWayRequestActions SetMethod(PolicyExpression method)
        {
            _writer.SetMethod(method);
            return this;
        }

        public ISendOneWayRequestActions SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null)
        {
            _document.SetHeader(name, existsAction, values);
            return this;
        }

        public ISendOneWayRequestActions SetBody(PolicyExpression body)
        {
            _document.SetBody(body);
            return this;
        }

        public ISendOneWayRequestActions AuthenticationCertificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null)
        {
            _document.AuthenticationCertificateInternal(thumbprint, certificate, body, password);
            return this;
        }

        public ISendOneWayRequestActions Proxy(PolicyExpression url, PolicyExpression? username = null, PolicyExpression? password = null)
        {
            _writer.Proxy(url, username, password);
            return this;
        }
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

    public void SendOneWayRequest(string? mode, string? timeout, Action writeRequest)
    {
        _xmlWriter.WriteStartElement("send-one-way-request");
        _xmlWriter.WriteAttributeStringOpt("mode", mode);
        _xmlWriter.WriteAttributeStringOpt("timeout", timeout);
        writeRequest();
        _xmlWriter.WriteEndElement();
    }
    internal void SendOneWayRequestUrl(string url)
    {
        _xmlWriter.WriteElementString("set-url", url);
    }
}
