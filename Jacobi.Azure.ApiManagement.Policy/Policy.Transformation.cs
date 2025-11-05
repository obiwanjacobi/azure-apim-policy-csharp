using System.Xml.Linq;

namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#transformation

internal interface ITransformation
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IPolicyFragment FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IPolicyFragment JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IPolicyFragment MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy</summary>
    IPolicyFragment RedirectContentUrls();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IPolicyFragment ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rewrite-uri-policy</summary>
    IPolicyFragment RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyFragment SetBody(PolicyExpression<string> body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyFragment SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IPolicyFragment SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-method-policy</summary>
    IPolicyFragment SetMethod(PolicyExpression<string> method);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IPolicyFragment SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy</summary>
    IPolicyFragment SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IPolicyFragment SetVariable(string name, PolicyExpression<string> value);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy</summary>
    IPolicyFragment XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xsl-transform-policy</summary>
    IPolicyFragment XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null);
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


partial class PolicyDocumentBase
{
    internal PolicyDocumentBase FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        // allowed in all sections

        Writer.FindAndReplace(from, to);
        return this;
    }

    internal PolicyDocumentBase JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null)
    {
        Writer.JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }

    internal PolicyDocumentBase MockResponse(int? statusCode = null, string? contentType = null)
    {
        Writer.MockResponse(statusCode.ToString(), contentType);
        return this;
    }

    internal PolicyDocumentBase RedirectContentUrls()
    {
        Writer.RedirectContentUrls();
        return this;
    }

    internal PolicyDocumentBase ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null)
    {
        Writer.ReturnResponse(responseVariableName, () => response(new ReturnResponseActions(this)));
        return this;
    }

    private sealed class ReturnResponseActions : IReturnResponseActions
    {
        private readonly PolicyDocumentBase _document;
        public ReturnResponseActions(PolicyDocumentBase document) { _document = document; }

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

    internal PolicyDocumentBase RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams)
    {
        Writer.RewriteUri(template, copyUnmatchedParams);
        return this;
    }

    internal PolicyDocumentBase SetBody(PolicyExpression<string> body)
    {
        Writer.SetBody(body);
        return this;
    }
    internal PolicyDocumentBase SetBody(LiquidTemplate body)
    {
        Writer.SetBody(body, liquidTemplate: true);
        return this;
    }

    internal PolicyDocumentBase SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null)
    {
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

    internal PolicyDocumentBase SetMethod(PolicyExpression<string> method)
    {
        Writer.SetMethod(method);
        return this;
    }

    internal PolicyDocumentBase SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        Writer.SetStatus(statusCode, reason);
        return this;
    }

    internal PolicyDocumentBase SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null)
    {
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

    internal PolicyDocumentBase SetVariable(string name, PolicyExpression<string> value)
    {
        Writer.SetVariable(name, value);
        return this;
    }

    internal PolicyDocumentBase XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null)
    {
        Writer.XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }

    internal PolicyDocumentBase XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null)
    {
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

partial class PolicyDocument
{
    IInbound IInbound.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        FindAndReplace(from, to);
        return this;
    }
    IBackend IBackend.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        FindAndReplace(from, to);
        return this;
    }
    IOutbound IOutbound.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        FindAndReplace(from, to);
        return this;
    }
    IOnError IOnError.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        FindAndReplace(from, to);
        return this;
    }

    IInbound IInbound.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }
    IOutbound IOutbound.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }
    IOnError IOnError.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }

    IInbound IInbound.MockResponse(int? statusCode, string? contentType)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        MockResponse(statusCode, contentType);
        return this;
    }
    IOutbound IOutbound.MockResponse(int? statusCode, string? contentType)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        MockResponse(statusCode, contentType);
        return this;
    }
    IOnError IOnError.MockResponse(int? statusCode, string? contentType)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        MockResponse(statusCode, contentType);
        return this;
    }

    IInbound IInbound.RedirectContentUrls()
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        RedirectContentUrls();
        return this;
    }
    IOutbound IOutbound.RedirectContentUrls()
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        RedirectContentUrls();
        return this;
    }

    IInbound IInbound.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        ReturnResponse(response, responseVariableName);
        return this;
    }
    IBackend IBackend.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        ReturnResponse(response, responseVariableName);
        return this;
    }
    IOutbound IOutbound.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        ReturnResponse(response, responseVariableName);
        return this;
    }
    IOnError IOnError.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        ReturnResponse(response, responseVariableName);
        return this;
    }

    IInbound IInbound.RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        RewriteUri(template, copyUnmatchedParams);
        return this;
    }

    IInbound IInbound.SetBody(PolicyExpression<string> body)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }
    IBackend IBackend.SetBody(PolicyExpression<string> body)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }
    IOutbound IOutbound.SetBody(PolicyExpression<string> body)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }
    IInbound IInbound.SetBody(LiquidTemplate body)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }
    IBackend IBackend.SetBody(LiquidTemplate body)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }
    IOutbound IOutbound.SetBody(LiquidTemplate body)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SetBody(body);
        return this;
    }

    IInbound IInbound.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetHeader(name, existsAction, values);
        return this;
    }
    IBackend IBackend.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetHeader(name, existsAction, values);
        return this;
    }
    IOutbound IOutbound.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SetHeader(name, existsAction, values);
        return this;
    }
    IOnError IOnError.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SetHeader(name, existsAction, values);
        return this;
    }

    IInbound IInbound.SetMethod(PolicyExpression<string> method)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetMethod(method);
        return this;
    }
    IOnError IOnError.SetMethod(PolicyExpression<string> method)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SetMethod(method);
        return this;
    }

    IInbound IInbound.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetStatus(statusCode, reason);
        return this;
    }
    IBackend IBackend.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetStatus(statusCode, reason);
        return this;
    }
    IOutbound IOutbound.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SetStatus(statusCode, reason);
        return this;
    }
    IOnError IOnError.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SetStatus(statusCode, reason);
        return this;
    }

    IInbound IInbound.SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetQueryParameter(name, values, existsAction);
        return this;
    }
    IBackend IBackend.SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetQueryParameter(name, values, existsAction);
        return this;
    }

    IInbound IInbound.SetVariable(string name, PolicyExpression<string> value)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        SetVariable(name, value);
        return this;
    }
    IBackend IBackend.SetVariable(string name, PolicyExpression<string> value)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        SetVariable(name, value);
        return this;
    }
    IOutbound IOutbound.SetVariable(string name, PolicyExpression<string> value)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        SetVariable(name, value);
        return this;
    }
    IOnError IOnError.SetVariable(string name, PolicyExpression<string> value)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        SetVariable(name, value);
        return this;
    }

    IInbound IInbound.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }
    IOutbound IOutbound.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }
    IOnError IOnError.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }

    IInbound IInbound.XslTransform(string xslt, Action<IXslTransformParameters>? parameters)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        XslTransform(xslt, parameters);
        return this;
    }
    IOutbound IOutbound.XslTransform(string xslt, Action<IXslTransformParameters>? parameters)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        XslTransform(xslt, parameters);
        return this;
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