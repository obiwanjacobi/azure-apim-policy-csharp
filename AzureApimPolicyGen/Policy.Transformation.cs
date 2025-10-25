namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#transformation

public interface ITransformation
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IPolicyDocument FindAndReplace(PolicyExpression from, PolicyExpression to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IPolicyDocument JsonToXml(PolicyExpression apply, PolicyExpression? considerAcceptHeader = null, bool? parseDate = null,
        PolicyExpression? namespaceSeparator = null, PolicyExpression? namespacePrefix = null, PolicyExpression? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IPolicyDocument MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy</summary>
    IPolicyDocument RedirectContentUrls();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IPolicyDocument ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rewrite-uri-policy</summary>
    IPolicyDocument RewriteUri(PolicyExpression template, bool? copyUnmatchedParams = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IPolicyDocument SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-method-policy</summary>
    IPolicyDocument SetMethod(PolicyExpression method);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IPolicyDocument SetStatus(PolicyExpression statusCode, PolicyExpression reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy</summary>
    IPolicyDocument SetQueryParameter(PolicyExpression name, Action<ISetQueryParameterValue> values, PolicyExpression? existsAction = null);
}

public interface IReturnResponseActions
{
    IReturnResponseActions SetStatus(PolicyExpression statusCode, PolicyExpression reason);
    IReturnResponseActions SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null);
    IReturnResponseActions SetBody(PolicyExpression body);
}

public interface ISetHeaderValue
{
    ISetHeaderValue Add(PolicyExpression value);
}

public interface ISetQueryParameterValue
{
    ISetQueryParameterValue Add(PolicyExpression value);
}


partial class PolicyDocument
{
    public IPolicyDocument FindAndReplace(PolicyExpression from, PolicyExpression to)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.FindAndReplace(from, to);
        return this;
    }

    public IPolicyDocument JsonToXml(PolicyExpression apply, PolicyExpression? considerAcceptHeader = null, bool? parseDate = null,
        PolicyExpression? namespaceSeparator = null, PolicyExpression? namespacePrefix = null, PolicyExpression? attributeBlockName = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }

    public IPolicyDocument MockResponse(int? statusCode = null, string? contentType = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.MockResponse(statusCode.ToString(), contentType);
        return this;
    }

    public IPolicyDocument RedirectContentUrls()
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound]);
        AssertScopes(PolicyScopes.All);
        Writer.RedirectContentUrls();
        return this;
    }

    public IPolicyDocument ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null)
    {
        AssertScopes(PolicyScopes.All);
        Writer.ReturnResponse(responseVariableName, () => response(new ReturnResponseActions(this)));
        return this;
    }

    private sealed class ReturnResponseActions : IReturnResponseActions
    {
        private readonly IPolicyDocument _document;
        public ReturnResponseActions(IPolicyDocument document) { _document = document; }

        public IReturnResponseActions SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null)
        {
            _document.SetHeader(name, existsAction, values);
            return this;
        }
        public IReturnResponseActions SetBody(PolicyExpression body)
        {
            _document.SetBody(body);
            return this;
        }

        public IReturnResponseActions SetStatus(PolicyExpression statusCode, PolicyExpression reason)
        {
            _document.SetStatus(statusCode, reason);
            return this;
        }
    }

    public IPolicyDocument RewriteUri(PolicyExpression template, bool? copyUnmatchedParams = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.RewriteUri(template, copyUnmatchedParams);
        return this;
    }

    public IPolicyDocument SetBody(PolicyExpression body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body);
        return this;
    }
    public IPolicyDocument SetBody(LiquidTemplate body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body, liquidTemplate: true);
        return this;
    }

    public IPolicyDocument SetHeader(PolicyExpression name, PolicyExpression? existsAction = null, Action<ISetHeaderValue>? values = null)
    {
        AssertScopes(PolicyScopes.All);
        Writer.SetHeader(name, existsAction, values is null ? null : () => values(new SetHeaderValue(Writer)));
        return this;
    }

    private sealed class SetHeaderValue : ISetHeaderValue
    {
        private readonly PolicyXmlWriter _writer;
        public SetHeaderValue(PolicyXmlWriter writer) { _writer = writer; }

        public ISetHeaderValue Add(PolicyExpression value)
        {
            _writer.SetHeaderValue(value);
            return this;
        }
    }

    public IPolicyDocument SetMethod(PolicyExpression method)
    {
        AssertSection([PolicySection.Inbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.SetMethod(method);
        return this;
    }

    public IPolicyDocument SetStatus(PolicyExpression statusCode, PolicyExpression reason)
    {
        AssertScopes(PolicyScopes.All);
        Writer.SetStatus(statusCode, reason);
        return this;
    }

    public IPolicyDocument SetQueryParameter(PolicyExpression name, Action<ISetQueryParameterValue> values, PolicyExpression? existsAction = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetQueryParameter(name, existsAction, () => values(new SetQueryParameterValue(Writer)));
        return this;
    }

    private sealed class SetQueryParameterValue : ISetQueryParameterValue
    {
        private readonly PolicyXmlWriter _writer;
        public SetQueryParameterValue(PolicyXmlWriter writer) { _writer = writer; }

        public ISetQueryParameterValue Add(PolicyExpression value)
        {
            _writer.SetQueryParameterValue(value);
            return this;
        }
    }
}

partial class PolicyXmlWriter
{
    public void FindAndReplace(string from, string to)
    {
        _xmlWriter.WriteStartElement("find-and-replace");
        _xmlWriter.WriteAttributeString("from", from);
        _xmlWriter.WriteAttributeString("to", to);
        _xmlWriter.WriteEndElement();
    }

    public void JsonToXml(string apply, string? considerAcceptHeader, bool? parseDate, string? namespaceSeparator,
        string? namespacePrefix, string? attributeBlockName)
    {
        _xmlWriter.WriteStartElement("json-to-xml");
        _xmlWriter.WriteAttributeString("apply", apply);
        _xmlWriter.WriteAttributeStringOpt("consider-accept-header", considerAcceptHeader);
        _xmlWriter.WriteAttributeStringOpt("parse-date", BoolValue(parseDate));
        _xmlWriter.WriteAttributeStringOpt("namespace-separator", namespaceSeparator);
        _xmlWriter.WriteAttributeStringOpt("namespace-prefix", namespacePrefix);
        _xmlWriter.WriteAttributeStringOpt("attribute-block-name", attributeBlockName);
        _xmlWriter.WriteEndElement();
    }

    public void MockResponse(string? statusCode, string? contentType)
    {
        _xmlWriter.WriteStartElement("mock-response");
        _xmlWriter.WriteAttributeStringOpt("status-code", statusCode);
        _xmlWriter.WriteAttributeStringOpt("content-type", contentType);
        _xmlWriter.WriteEndElement();
    }

    public void RedirectContentUrls()
    {
        _xmlWriter.WriteStartElement("redirect-content-urls");
        _xmlWriter.WriteEndElement();
    }

    public void ReturnResponse(string? responseVariableName, Action writeResponse)
    {
        _xmlWriter.WriteStartElement("return-response");
        _xmlWriter.WriteAttributeStringOpt("response-variable-name", responseVariableName);
        writeResponse();
        _xmlWriter.WriteEndElement();
    }

    public void RewriteUri(string template, bool? copyUnmatchedParams)
    {
        _xmlWriter.WriteStartElement("rewrite-uri");
        _xmlWriter.WriteAttributeString("template", template);
        _xmlWriter.WriteAttributeStringOpt("copy-unmatched-params", BoolValue(copyUnmatchedParams));
        _xmlWriter.WriteEndElement();
    }

    public void SetBody(string body, bool liquidTemplate = false)
    {
        _xmlWriter.WriteStartElement("set-body");
        if (liquidTemplate)
            _xmlWriter.WriteAttributeString("template", "liquid");
        _xmlWriter.WriteString(body);
        _xmlWriter.WriteEndElement();
    }

    public void SetHeader(string name, string? existsAction, Action? writeValues)
    {
        _xmlWriter.WriteStartElement("set-header");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeStringOpt("exists-action", existsAction);
        if (writeValues is not null) writeValues();
        _xmlWriter.WriteEndElement();
    }
    internal void SetHeaderValue(string value)
    {
        _xmlWriter.WriteElementString("value", value);
    }

    public void SetMethod(string method)
    {
        _xmlWriter.WriteElementString("set-method", method);
    }

    public void SetStatus(string statusCode, string reason)
    {
        _xmlWriter.WriteStartElement("set-status");
        _xmlWriter.WriteAttributeString("status-code", statusCode);
        _xmlWriter.WriteAttributeString("reason", reason);
        _xmlWriter.WriteEndElement();
    }

    public void SetQueryParameter(string name, string? existsAction, Action writeValues)
    {
        _xmlWriter.WriteStartElement("set-query-parameter");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeStringOpt("exists-action", existsAction);
        writeValues();
        _xmlWriter.WriteEndElement();
    }
    internal void SetQueryParameterValue(string value)
    {
        _xmlWriter.WriteElementString("value", value);
    }
}