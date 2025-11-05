namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#routing

public interface IRouting
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/forward-request-policy</summary>
    IPolicyFragment ForwardRequest(HttpVersion? httpVersion = null, PolicyExpression<int>? timeoutSeconds = null, PolicyExpression<int>? timeoutMilliseconds = null, PolicyExpression<int>? continueTimeout = null, bool? followRedirects = null, bool? bufferRequestBody = null, bool? bufferResponse = null, bool? failOnErrorStatusCode = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/proxy-policy</summary>
    IPolicyFragment Proxy(PolicyExpression<string> url, PolicyExpression<string>? username = null, PolicyExpression<string>? password = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IPolicyFragment SetBackendService(PolicyExpression<string> baseUrl);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IPolicyFragment SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition = null, PolicyExpression<string>? sfServiceInstanceName = null, PolicyExpression<string>? sfPartitionKey = null, PolicyExpression<string>? sfListenerName = null);
}

public enum HttpVersion
{
    /// <summary>Not set</summary>
    None = 0x00,
    /// <summary>Use Http/1</summary>
    Http1 = 0x01,
    /// <summary>Use Http/2</summary>
    Http2 = 0x02,
    /// <summary>Favor Http/2 over Http/1</summary>
    Http2_1 = 0x03
}

partial class PolicyDocumentBase
{
    internal PolicyDocumentBase ForwardRequest(HttpVersion? httpVersion, PolicyExpression<int>? timeoutSeconds, PolicyExpression<int>? timeoutMilliseconds, PolicyExpression<int>? continueTimeout, bool? followRedirects, bool? bufferRequestBody, bool? bufferResponse, bool? failOnErrorStatusCode)
    {
        Writer.ForwardRequest(HttpVersionToString(httpVersion), timeoutSeconds, timeoutMilliseconds, continueTimeout,
            followRedirects, bufferRequestBody, bufferResponse, failOnErrorStatusCode);
        return this;

        static string? HttpVersionToString(HttpVersion? httpVersion)
            => httpVersion switch
            {
                HttpVersion.Http1 => "1",
                HttpVersion.Http2 => "2",
                HttpVersion.Http2_1 => "2or1",
                _ => null
            };
    }

    internal PolicyDocumentBase Proxy(PolicyExpression<string> url, PolicyExpression<string>? username, PolicyExpression<string>? password)
    {
        Writer.Proxy(url, username, password);
        return this;
    }

    internal PolicyDocumentBase SetBackendService(PolicyExpression<string> baseUrl)
    {
        Writer.SetBackendService(baseUrl);
        return this;
    }

    internal PolicyDocumentBase SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition = null, PolicyExpression<string>? sfServiceInstanceName = null, PolicyExpression<string>? sfPartitionKey = null, PolicyExpression<string>? sfListenerName = null)
    {
        Writer.SetBackendService(backendId, sfResolveCondition, sfServiceInstanceName, sfPartitionKey, sfListenerName);
        return this;
    }
}

partial class PolicyDocument
{
    IBackend IBackend.ForwardRequest(HttpVersion? httpVersion, PolicyExpression<int>? timeoutSeconds, PolicyExpression<int>? timeoutMilliseconds, PolicyExpression<int>? continueTimeout, bool? followRedirects, bool? bufferRequestBody, bool? bufferResponse, bool? failOnErrorStatusCode)
    {
        AssertSection(PolicySection.Backend);
        ForwardRequest(httpVersion, timeoutSeconds, timeoutMilliseconds, continueTimeout, followRedirects, bufferRequestBody, bufferResponse, failOnErrorStatusCode);
        return this;
    }

    IInbound IInbound.Proxy(PolicyExpression<string> url, PolicyExpression<string>? username, PolicyExpression<string>? password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Proxy(url, username, password);
        return this;
    }

    IInbound IInbound.SetBackendService(PolicyExpression<string> baseUrl)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetBackendService(baseUrl);
        return this;
    }
    IBackend IBackend.SetBackendService(PolicyExpression<string> baseUrl)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetBackendService(baseUrl);
        return this;
    }

    IInbound IInbound.SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition, PolicyExpression<string>? sfServiceInstanceName, PolicyExpression<string>? sfPartitionKey, PolicyExpression<string>? sfListenerName)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetBackendService(backendId, sfResolveCondition, sfServiceInstanceName, sfPartitionKey, sfListenerName);
        return this;
    }
    IBackend IBackend.SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition, PolicyExpression<string>? sfServiceInstanceName, PolicyExpression<string>? sfPartitionKey, PolicyExpression<string>? sfListenerName)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetBackendService(backendId, sfResolveCondition, sfServiceInstanceName, sfPartitionKey, sfListenerName);
        return this;
    }
}

partial class PolicyXmlWriter
{
    public void ForwardRequest(string? httpVersion, string? timeoutSeconds, string? timeoutMilliseconds, string? continueTimout,
        bool? followRedirects, bool? bufferRequestBody, bool? bufferResponse, bool? failOnErrorStatusCode)
    {
        _xmlWriter.WriteStartElement("forward-request");
        _xmlWriter.WriteAttributeStringOpt("http-version", httpVersion);
        _xmlWriter.WriteAttributeStringOpt("timeout", timeoutSeconds);
        _xmlWriter.WriteAttributeStringOpt("timeout-ms", timeoutMilliseconds);
        _xmlWriter.WriteAttributeStringOpt("continue-timeout", continueTimout);
        _xmlWriter.WriteAttributeStringOpt("follow-redirects", BoolValue(followRedirects));
        _xmlWriter.WriteAttributeStringOpt("buffer-request-body", BoolValue(bufferRequestBody));
        _xmlWriter.WriteAttributeStringOpt("buffer-response", BoolValue(bufferResponse));
        _xmlWriter.WriteAttributeStringOpt("fail-on-error-status-code", BoolValue(failOnErrorStatusCode));
        _xmlWriter.WriteEndElement();
    }

    public void Proxy(string url, string? username, string? password)
    {
        _xmlWriter.WriteStartElement("proxy");
        _xmlWriter.WriteAttributeString("url", url);
        _xmlWriter.WriteAttributeStringOpt("username", username);
        _xmlWriter.WriteAttributeStringOpt("password", password);
        _xmlWriter.WriteEndElement();
    }

    public void SetBackendService(string baseUrl)
    {
        _xmlWriter.WriteStartElement("set-backend-service");
        _xmlWriter.WriteAttributeString("base-url", baseUrl);
        _xmlWriter.WriteEndElement();
    }
    public void SetBackendService(string backendId, string? sfResolveCondition, string? sfServiceInstanceName, string? sfPartitionKey, string? sfListenerName)
    {
        _xmlWriter.WriteStartElement("set-backend-service");
        _xmlWriter.WriteAttributeString("backend-id", backendId);
        _xmlWriter.WriteAttributeStringOpt("sf-resolve-condition", sfResolveCondition);
        _xmlWriter.WriteAttributeStringOpt("sf-service-instance-name", sfServiceInstanceName);
        _xmlWriter.WriteAttributeStringOpt("sf-partition-key", sfPartitionKey);
        _xmlWriter.WriteAttributeStringOpt("sf-listener-name", sfListenerName);
        _xmlWriter.WriteEndElement();
    }
}