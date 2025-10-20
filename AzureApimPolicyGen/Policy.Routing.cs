namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#routing

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

partial class PolicyDocument
{

    public IPolicyDocument ForwardRequest(HttpVersion? httpVersion = null,
        PolicyExpression? timeoutSeconds = null, PolicyExpression? timeoutMilliseconds = null, PolicyExpression? continueTimeout = null,
        bool? followRedirects = null, bool? bufferRequestBody = null, bool? bufferResponse = null, bool? failOnErrorStatusCode = null)
    {
        AssertSection(PolicySection.Backend);
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
}