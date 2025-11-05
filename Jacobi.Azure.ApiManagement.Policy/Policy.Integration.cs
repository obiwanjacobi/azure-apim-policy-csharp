namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#integration-and-external-communication

public interface IIntegration
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyFragment PublishToDapr(PolicyExpression<string> message, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression<int>? timeoutSeconds = null, string? contentType = null, bool? ignoreError = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IPolicyFragment PublishToDapr(LiquidTemplate template, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression<int>? timeoutSeconds = null, string? contentType = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IPolicyFragment SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-service-bus-message-policy</summary>
    IPolicyFragment SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties = null, PolicyExpression<string>? queueName = null, PolicyExpression<string>? topicName = null, PolicyExpression<string>? clientId = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-request-policy</summary>
    IPolicyFragment SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-dapr-policy</summary>
    IPolicyFragment SetBackendService(PolicyExpression<string> daprAppId, PolicyExpression<string> daprMethod, PolicyExpression<string>? daprNamespace = null);
}

public interface ISendRequestActions
{
    ISendRequestActions SetUrl(PolicyExpression<string> url);
    ISendRequestActions SetMethod(PolicyExpression<string> method);
    ISendRequestActions SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);
    ISendRequestActions SetBody(PolicyExpression<string> body);
    ISendRequestActions AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body = null, PolicyExpression<string>? password = null);
    ISendRequestActions AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = false);
    ISendRequestActions Proxy(PolicyExpression<string> url, PolicyExpression<string>? username = null, PolicyExpression<string>? password = null);
}

public interface ISendServiceBusMessageProperties
{
    ISendServiceBusMessageProperties Add(string name, string value);
}

partial class PolicyDocumentBase
{
    internal PolicyDocumentBase PublishToDapr(PolicyExpression<string> message, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        Writer.PublishToDapr(message, topic, null, pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }
    internal PolicyDocumentBase PublishToDapr(LiquidTemplate template, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        Writer.PublishToDapr(template, topic, "Liquid", pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }

    internal PolicyDocumentBase SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null)
    {
        Writer.SendOneWayRequest(mode, timeoutSeconds, () => request(new SendRequestActions(this, Writer)));
        return this;
    }

    private sealed class SendRequestActions : ISendRequestActions
    {
        private readonly PolicyXmlWriter _writer;
        private readonly PolicyDocumentBase _document;
        public SendRequestActions(PolicyDocumentBase document, PolicyXmlWriter writer)
        {
            _writer = writer;
            _document = document;
        }

        public ISendRequestActions SetUrl(PolicyExpression<string> url)
        {
            _writer.SendRequestUrl(url);
            return this;
        }

        public ISendRequestActions SetMethod(PolicyExpression<string> method)
        {
            _writer.SetMethod(method);
            return this;
        }

        public ISendRequestActions SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null)
        {
            _document.SetHeader(name, existsAction, values);
            return this;
        }

        public ISendRequestActions SetBody(PolicyExpression<string> body)
        {
            _document.SetBody(body);
            return this;
        }

        public ISendRequestActions AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body = null, PolicyExpression<string>? password = null)
        {
            _document.AuthenticationCertificate(thumbprint, certificate, body, password);
            return this;
        }

        public ISendRequestActions AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = false)
        {
            _document.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
            return this;
        }

        public ISendRequestActions Proxy(PolicyExpression<string> url, PolicyExpression<string>? username = null, PolicyExpression<string>? password = null)
        {
            _writer.Proxy(url, username, password);
            return this;
        }
    }

    internal PolicyDocumentBase SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties, PolicyExpression<string>? queueName = null, PolicyExpression<string>? topicName = null, PolicyExpression<string>? clientId = null)
    {
        if (queueName is null && topicName is null)
            throw new ArgumentException($"Either {nameof(queueName)} or {nameof(topicName)} must be filled.", $"{nameof(queueName)}+{nameof(topicName)}");
        if (queueName is not null && topicName is not null)
            throw new ArgumentException($"Either {nameof(queueName)} or {nameof(topicName)} must be filled. But not both.", $"{nameof(queueName)}+{nameof(topicName)}");

        Action? writeActions = messageProperties is null ? null : () => messageProperties(new SendServiceBusMessageProperties(Writer));
        Writer.SendServiceBusMessage(@namespace, queueName, topicName, clientId, message, writeActions);
        return this;
    }

    private sealed class SendServiceBusMessageProperties : ISendServiceBusMessageProperties
    {
        private readonly PolicyXmlWriter _writer;
        public SendServiceBusMessageProperties(PolicyXmlWriter writer) { _writer = writer; }

        public ISendServiceBusMessageProperties Add(string name, string value)
        {
            _writer.SendServiceBusMessageProperty(name, value);
            return this;
        }
    }

    internal PolicyDocumentBase SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null)
    {
        Writer.SendRequest(responseVariableName, mode, timeoutSeconds, ignoreError, () => request(new SendRequestActions(this, Writer)));
        return this;
    }

    internal PolicyDocumentBase SetBackendService(PolicyExpression<string> daprAppId, PolicyExpression<string> daprMethod, PolicyExpression<string>? daprNamespace)
    {
        Writer.SetBackendService(daprAppId, daprMethod, daprNamespace);
        return this;
    }
}

partial class PolicyDocument
{
    IInbound IInbound.PublishToDapr(PolicyExpression<string> message, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        PublishToDapr(message, topic, pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }
    IInbound IInbound.PublishToDapr(LiquidTemplate template, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        PublishToDapr(template, topic, pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }

    IInbound IInbound.SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SendOneWayRequest(request, mode, timeoutSeconds);
        return this;
    }
    IBackend IBackend.SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SendOneWayRequest(request, mode, timeoutSeconds);
        return this;
    }
    IOutbound IOutbound.SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SendOneWayRequest(request, mode, timeoutSeconds);
        return this;
    }
    IOnError IOnError.SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SendOneWayRequest(request, mode, timeoutSeconds);
        return this;
    }

    IInbound IInbound.SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties, PolicyExpression<string>? queueName, PolicyExpression<string>? topicName, PolicyExpression<string>? clientId)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        SendServiceBusMessage(@namespace, message, messageProperties, queueName, topicName, clientId);
        return this;
    }
    IOutbound IOutbound.SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties, PolicyExpression<string>? queueName, PolicyExpression<string>? topicName, PolicyExpression<string>? clientId)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        SendServiceBusMessage(@namespace, message, messageProperties, queueName, topicName, clientId);
        return this;
    }
    IOnError IOnError.SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties, PolicyExpression<string>? queueName, PolicyExpression<string>? topicName, PolicyExpression<string>? clientId)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        SendServiceBusMessage(@namespace, message, messageProperties, queueName, topicName, clientId);
        return this;
    }

    IInbound IInbound.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
        return this;
    }
    IBackend IBackend.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
        return this;
    }
    IOutbound IOutbound.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
        return this;
    }
    IOnError IOnError.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
        return this;
    }

    IInbound IInbound.SetBackendService(PolicyExpression<string> daprAppId, PolicyExpression<string> daprMethod, PolicyExpression<string>? daprNamespace)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        SetBackendService(daprAppId, daprMethod, daprNamespace);
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

    public void SendOneWayRequest(string? mode, string? timeout, Action writeRequest)
    {
        _xmlWriter.WriteStartElement("send-one-way-request");
        _xmlWriter.WriteAttributeStringOpt("mode", mode);
        _xmlWriter.WriteAttributeStringOpt("timeout", timeout);
        writeRequest();
        _xmlWriter.WriteEndElement();
    }
    internal void SendRequestUrl(string url)
    {
        _xmlWriter.WriteElementString("set-url", url);
    }

    public void SendServiceBusMessage(string @namespace, string? queueName, string? topicName, string? clientId, string message, Action? writeProperties)
    {
        _xmlWriter.WriteStartElement("send-service-bus-message");
        _xmlWriter.WriteAttributeString("namespace", @namespace);
        _xmlWriter.WriteAttributeStringOpt("queue-name", queueName);
        _xmlWriter.WriteAttributeStringOpt("topic-name", topicName);
        _xmlWriter.WriteAttributeStringOpt("client-id", clientId);
        if (!String.IsNullOrEmpty(message))
        {
            _xmlWriter.WriteElementString("payload", message);
        }
        if (writeProperties is not null)
        {
            _xmlWriter.WriteStartElement("message-properties");
            writeProperties();
            _xmlWriter.WriteEndElement();
        }
        _xmlWriter.WriteEndElement();
    }
    internal void SendServiceBusMessageProperty(string name, string value)
    {
        _xmlWriter.WriteStartElement("message-property");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteString(value);
        _xmlWriter.WriteEndElement();
    }

    public void SendRequest(string responseVariableName, string? mode, string? timeout, bool? ignoreError, Action writeRequest)
    {
        _xmlWriter.WriteStartElement("send-request");
        _xmlWriter.WriteAttributeString("response-variable-name", responseVariableName);
        _xmlWriter.WriteAttributeStringOpt("mode", mode);
        _xmlWriter.WriteAttributeStringOpt("timeout", timeout);
        _xmlWriter.WriteAttributeStringOpt("ignore-error", BoolValue(ignoreError));
        writeRequest();
        _xmlWriter.WriteEndElement();
    }

    public void SetBackendService(string daprAppId, string daprMethod, string? daprNamespace)
    {
        _xmlWriter.WriteStartElement("set-backend-service");
        _xmlWriter.WriteAttributeString("backend-id", "dapr");
        _xmlWriter.WriteAttributeString("dapr-app-id", daprAppId);
        _xmlWriter.WriteAttributeString("dapr-method", daprMethod);
        _xmlWriter.WriteAttributeStringOpt("dapr-namespace", daprNamespace);
        _xmlWriter.WriteEndElement();
    }
}
