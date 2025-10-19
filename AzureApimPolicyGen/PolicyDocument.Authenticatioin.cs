namespace AzureApimPolicyGen;

public interface IAuthentication
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    IPolicyDocument Basic(string username, string password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    IPolicyDocument Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body = null, PolicyExpression? password = null);

    // TODO: use in 'send-request'
    // https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy#use-managed-identity-in-send-request-policy
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    IPolicyDocument ManagedIdentity(PolicyExpression resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool ignoreError = false);
}

partial class PolicyDocument : IAuthentication
{
    public IAuthentication Authentication => (IAuthentication)this;

    IPolicyDocument IAuthentication.Basic(string username, string password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.AuthenticationBasic(username, password);
        return this;
    }

    IPolicyDocument IAuthentication.Certificate(PolicyExpression thumbprint, PolicyExpression certificate, PolicyExpression? body, PolicyExpression? password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        if (!String.IsNullOrEmpty(thumbprint) && !String.IsNullOrEmpty(certificate))
            throw new ArgumentException("Specify either a thumbprint or a certificate.  Not both.", $"{nameof(thumbprint)}+{nameof(certificate)}");

        Writer.AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    IPolicyDocument IAuthentication.ManagedIdentity(PolicyExpression resource, string? clientId, PolicyVariable? outputTokenVariableName, bool ignoreError)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        // TODO: check variable exists
        Writer.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }
}