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

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);
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

    public void SetBody(string body, bool liquidTemplate = false)
    {
        _xmlWriter.WriteStartElement("set-body");
        if (liquidTemplate)
            _xmlWriter.WriteAttributeString("template", "liquid");
        _xmlWriter.WriteString(body);
        _xmlWriter.WriteEndElement();
    }
}