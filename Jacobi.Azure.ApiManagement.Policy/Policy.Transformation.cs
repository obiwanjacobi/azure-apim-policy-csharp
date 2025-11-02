using System.Xml.Linq;

namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#transformation

internal interface ITransformation
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IPolicyDocument FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IPolicyDocument JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IPolicyDocument MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy</summary>
    IPolicyDocument RedirectContentUrls();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IPolicyDocument ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rewrite-uri-policy</summary>
    IPolicyDocument RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression<string> body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IPolicyDocument SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-method-policy</summary>
    IPolicyDocument SetMethod(PolicyExpression<string> method);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IPolicyDocument SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy</summary>
    IPolicyDocument SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IPolicyDocument SetVariable(string name, PolicyExpression<string> value);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy</summary>
    IPolicyDocument XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xsl-transform-policy</summary>
    IPolicyDocument XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null);
}

public interface IReturnResponseActions
{
    IReturnResponseActions SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);
    IReturnResponseActions SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);
    IReturnResponseActions SetBody(PolicyExpression<string> body);
}

public interface ISetHeaderValue
{
    ISetHeaderValue Add(params IEnumerable<PolicyExpression<string>> values);
}

public interface ISetQueryParameterValue
{
    ISetQueryParameterValue Add(params IEnumerable<PolicyExpression<string>> values);
}

public interface IXslTransformParameters
{
    IXslTransformParameters Add(string name, string value);
}


partial class PolicyDocument
{
    IInbound IInbound.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
        => FindAndReplace(from, to);
    IBackend IBackend.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
        => FindAndReplace(from, to);
    IOutbound IOutbound.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
        => FindAndReplace(from, to);
    IOnError IOnError.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
        => FindAndReplace(from, to);
    private PolicyDocument FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.FindAndReplace(from, to);
        return this;
    }

    IInbound IInbound.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
        => JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
    IOutbound IOutbound.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
        => JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
    IOnError IOnError.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
        => JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
    private PolicyDocument JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }

    IInbound IInbound.MockResponse(int? statusCode, string? contentType)
        => MockResponse(statusCode, contentType);
    IOutbound IOutbound.MockResponse(int? statusCode, string? contentType)
        => MockResponse(statusCode, contentType);
    IOnError IOnError.MockResponse(int? statusCode, string? contentType)
        => MockResponse(statusCode, contentType);
    private PolicyDocument MockResponse(int? statusCode = null, string? contentType = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.MockResponse(statusCode.ToString(), contentType);
        return this;
    }

    IInbound IInbound.RedirectContentUrls()
        => RedirectContentUrls();
    IOutbound IOutbound.RedirectContentUrls()
        => RedirectContentUrls();
    private PolicyDocument RedirectContentUrls()
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound]);
        AssertScopes(PolicyScopes.All);
        Writer.RedirectContentUrls();
        return this;
    }

    IInbound IInbound.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
        => ReturnResponse(response, responseVariableName);
    IBackend IBackend.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
        => ReturnResponse(response, responseVariableName);
    IOutbound IOutbound.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
        => ReturnResponse(response, responseVariableName);
    IOnError IOnError.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
        => ReturnResponse(response, responseVariableName);
    private PolicyDocument ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null)
    {
        AssertScopes(PolicyScopes.All);
        Writer.ReturnResponse(responseVariableName, () => response(new ReturnResponseActions(this)));
        return this;
    }

    private sealed class ReturnResponseActions : IReturnResponseActions
    {
        private readonly PolicyDocument _document;
        public ReturnResponseActions(PolicyDocument document) { _document = document; }

        public IReturnResponseActions SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null)
        {
            _document.SetHeader(name, existsAction, values);
            return this;
        }

        bool _setBodyCalled = false;
        public IReturnResponseActions SetBody(PolicyExpression<string> body)
        {
            if (_setBodyCalled)
                throw new InvalidOperationException("SetBody can be called only once.");
            _document.SetBody(body);
            _setBodyCalled = true;
            return this;
        }

        bool _setStatusCalled = false;
        public IReturnResponseActions SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
        {
            if (_setStatusCalled)
                throw new InvalidOperationException("SetStatus can be called only once.");
            _document.SetStatus(statusCode, reason);
            _setStatusCalled = true;
            return this;
        }
    }

    IInbound IInbound.RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.RewriteUri(template, copyUnmatchedParams);
        return this;
    }

    IInbound IInbound.SetBody(PolicyExpression<string> body)
        => SetBody(body);
    IBackend IBackend.SetBody(PolicyExpression<string> body)
        => SetBody(body);
    IOutbound IOutbound.SetBody(PolicyExpression<string> body)
        => SetBody(body);
    private PolicyDocument SetBody(PolicyExpression<string> body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body);
        return this;
    }
    IInbound IInbound.SetBody(LiquidTemplate body)
        => SetBody(body);
    IBackend IBackend.SetBody(LiquidTemplate body)
        => SetBody(body);
    IOutbound IOutbound.SetBody(LiquidTemplate body)
        => SetBody(body);
    private PolicyDocument SetBody(LiquidTemplate body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body, liquidTemplate: true);
        return this;
    }

    IInbound IInbound.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
        => SetHeader(name, existsAction, values);
    IBackend IBackend.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
        => SetHeader(name, existsAction, values);
    IOutbound IOutbound.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
        => SetHeader(name, existsAction, values);
    IOnError IOnError.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
        => SetHeader(name, existsAction, values);
    private PolicyDocument SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null)
    {
        AssertScopes(PolicyScopes.All);
        Writer.SetHeader(name, existsAction, values is null ? null : () => values(new SetHeaderValue(Writer)));
        return this;
    }

    private sealed class SetHeaderValue : ISetHeaderValue
    {
        private readonly PolicyXmlWriter _writer;
        public SetHeaderValue(PolicyXmlWriter writer) { _writer = writer; }

        public ISetHeaderValue Add(params IEnumerable<PolicyExpression<string>> values)
        {
            foreach (var value in values)
                _writer.SetHeaderValue(value);
            return this;
        }
    }

    IInbound IInbound.SetMethod(PolicyExpression<string> method)
        => SetMethod(method);
    IOnError IOnError.SetMethod(PolicyExpression<string> method)
        => SetMethod(method);
    private PolicyDocument SetMethod(PolicyExpression<string> method)
    {
        AssertSection([PolicySection.Inbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.SetMethod(method);
        return this;
    }

    IInbound IInbound.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
        => SetStatus(statusCode, reason);
    IBackend IBackend.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
        => SetStatus(statusCode, reason);
    IOutbound IOutbound.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
        => SetStatus(statusCode, reason);
    IOnError IOnError.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
        => SetStatus(statusCode, reason);
    private PolicyDocument SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        AssertScopes(PolicyScopes.All);
        Writer.SetStatus(statusCode, reason);
        return this;
    }

    IInbound IInbound.SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction)
        => SetQueryParameter(name, values, existsAction);
    IBackend IBackend.SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction)
        => SetQueryParameter(name, values, existsAction);
    private PolicyDocument SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null)
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

        public ISetQueryParameterValue Add(params IEnumerable<PolicyExpression<string>> values)
        {
            foreach (var value in values)
                _writer.SetQueryParameterValue(value);
            return this;
        }
    }

    IInbound IInbound.SetVariable(string name, PolicyExpression<string> value)
        => SetVariable(name, value);
    IBackend IBackend.SetVariable(string name, PolicyExpression<string> value)
        => SetVariable(name, value);
    IOutbound IOutbound.SetVariable(string name, PolicyExpression<string> value)
        => SetVariable(name, value);
    IOnError IOnError.SetVariable(string name, PolicyExpression<string> value)
        => SetVariable(name, value);
    private PolicyDocument SetVariable(string name, PolicyExpression<string> value)
    {
        AssertScopes(PolicyScopes.All);
        Writer.SetVariable(name, value);
        return this;
    }

    IInbound IInbound.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
        => XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
    IOutbound IOutbound.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
        => XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
    IOnError IOnError.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
        => XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
    private PolicyDocument XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Writer.XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }

    IInbound IInbound.XslTransform(string xslt, Action<IXslTransformParameters>? parameters)
        => XslTransform(xslt, parameters);
    IOutbound IOutbound.XslTransform(string xslt, Action<IXslTransformParameters>? parameters)
        => XslTransform(xslt, parameters);
    private PolicyDocument XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound]);
        AssertScopes(PolicyScopes.All);
        Action? writeParams = parameters is null ? null : () => parameters(new XslTransformParameters(Writer));
        var xsltDoc = XDocument.Parse(xslt);
        Writer.XslTransform(xsltDoc, writeParams);
        return this;
    }

    private sealed class XslTransformParameters : IXslTransformParameters
    {
        private readonly PolicyXmlWriter _writer;
        public XslTransformParameters(PolicyXmlWriter writer) { _writer = writer; }

        public IXslTransformParameters Add(string name, string value)
        {
            _writer.XslTransformParameter(name, value);
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

    public void SetVariable(string name, string value)
    {
        _xmlWriter.WriteStartElement("set-variable");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeString("value", value);
        _xmlWriter.WriteEndElement();
    }

    public void XmlToJson(string kind, string apply, string? considerAcceptHeader, string? alwaysArrayChildElements)
    {
        _xmlWriter.WriteStartElement("xml-to-json");
        _xmlWriter.WriteAttributeString("kind", kind);
        _xmlWriter.WriteAttributeString("apply", apply);
        _xmlWriter.WriteAttributeStringOpt("consider-accept-header", considerAcceptHeader);
        _xmlWriter.WriteAttributeStringOpt("always-array-child-elements", alwaysArrayChildElements);
        _xmlWriter.WriteEndElement();
    }

    public void XslTransform(XDocument xslt, Action? writeParams)
    {
        _xmlWriter.WriteStartElement("xsl-transform");
        if (writeParams is not null) writeParams();
        xslt.Root!.WriteTo(_xmlWriter);
        _xmlWriter.WriteEndElement();
    }
    internal void XslTransformParameter(string name, string value)
    {
        _xmlWriter.WriteStartElement("parameter");
        _xmlWriter.WriteAttributeString("parameter-name", name);
        _xmlWriter.WriteString(value);
        _xmlWriter.WriteEndElement();
    }
}