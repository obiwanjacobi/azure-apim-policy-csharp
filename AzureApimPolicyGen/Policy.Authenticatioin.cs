namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#authentication-and-authorization

public interface ICheckHeaderValues
{
    ICheckHeaderValues Add(string value);
}


partial class PolicyDocument
{
    public IPolicyDocument AuthenticationBasic(PolicyExpression username, PolicyExpression password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.AuthenticationBasic(username, password);
        return this;
    }

    public IPolicyDocument AuthenticationCertificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body, PolicyExpression? password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        if (!String.IsNullOrEmpty(thumbprint) && !String.IsNullOrEmpty(certificate))
            throw new ArgumentException("Specify either a thumbprint or a certificate.  Not both.", $"{nameof(thumbprint)}+{nameof(certificate)}");

        Writer.AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    public IPolicyDocument AuthenticationManagedIdentity(PolicyExpression resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        // TODO: check variable exists
        Writer.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }

    public IPolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode,
        PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? writeValues = values is null ? null : () => values(new CheckHeaderValues(Writer));
        Writer.CheckHeader(name, failedCheckHttpCode, failedCheckErrorMessage, ignoreCase, writeValues);
        return this;
    }

    private sealed class CheckHeaderValues : ICheckHeaderValues
    {
        private readonly PolicyXmlWriter _writer;
        internal CheckHeaderValues(PolicyXmlWriter writer) => _writer = writer;

        public ICheckHeaderValues Add(string value)
        {
            _writer.CheckHeaderValue(value);
            return this;
        }
    }
}

partial class PolicyXmlWriter
{
    public void AuthenticationBasic(string username, string password)
    {
        _xmlWriter.WriteStartElement("authentication-basic");
        _xmlWriter.WriteAttributeString("username", username);
        _xmlWriter.WriteAttributeString("password", password);
        _xmlWriter.WriteEndElement();
    }

    public void AuthenticationCertificate(string? thumbprint, string? certificate, string? body, string? password)
    {
        _xmlWriter.WriteStartElement("authentication-certificate");
        _xmlWriter.WriteAttributeStringOpt("thumbprint", thumbprint);
        _xmlWriter.WriteAttributeStringOpt("certificate", certificate);
        _xmlWriter.WriteAttributeStringOpt("body", body);
        _xmlWriter.WriteAttributeStringOpt("password", password);
        _xmlWriter.WriteEndElement();
    }

    public void AuthenticationManagedIdentity(string resource, string? clientId, string? outputTokenVariableName, bool? ignoreError)
    {
        _xmlWriter.WriteStartElement("authentication-managed-identity");
        _xmlWriter.WriteAttributeString("resource", resource);
        _xmlWriter.WriteAttributeStringOpt("client-id", clientId);
        _xmlWriter.WriteAttributeStringOpt("output-token-variable-name", outputTokenVariableName);
        _xmlWriter.WriteAttributeString("ignore-error", BoolValue(ignoreError));
        _xmlWriter.WriteEndElement();
    }

    public void CheckHeader(string name, string failedCheckHttpCode, string failedCheckErrorMessage,
        string ignoreCase, Action? writeValues)
    {
        _xmlWriter.WriteStartElement("check-header");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeString("failed-check-http-code", failedCheckHttpCode);
        _xmlWriter.WriteAttributeString("failed-check-error-message", failedCheckErrorMessage);
        _xmlWriter.WriteAttributeString("ignore-case", ignoreCase);
        if (writeValues is not null) writeValues();
        _xmlWriter.WriteEndElement();
    }
    internal void CheckHeaderValue(string value)
    {
        _xmlWriter.WriteElementString("value", value);
    }
}