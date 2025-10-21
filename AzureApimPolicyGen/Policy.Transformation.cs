namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#transformation

public interface ITransformation
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IPolicyDocument FindAndReplace(PolicyExpression from, PolicyExpression to);



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

    public void SetBody(string body, bool liquidTemplate = false)
    {
        _xmlWriter.WriteStartElement("set-body");
        if (liquidTemplate)
            _xmlWriter.WriteAttributeString("template", "liquid");
        _xmlWriter.WriteString(body);
        _xmlWriter.WriteEndElement();
    }
}