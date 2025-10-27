namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#authentication-and-authorization

public interface IAuthentication
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    IPolicyDocument AuthenticationBasic(PolicyExpression<string> username, PolicyExpression<string> password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    IPolicyDocument AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body = null, PolicyExpression<string>? password = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    IPolicyDocument AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = false);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/check-header-policy</summary>
    IPolicyDocument CheckHeader(PolicyExpression<string> name, PolicyExpression<int> failedCheckHttpCode, PolicyExpression<string> failedCheckErrorMessage, PolicyExpression<bool> ignoreCase, Action<ICheckHeaderValues>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/get-authorization-context-policy</summary>
    IPolicyDocument GetAuthorizationContext(PolicyExpression<string> providerId, PolicyExpression<string> authorizationId, PolicyVariable contextVariableName, PolicyExpression<string>? identity = null, PolicyExpression<bool>? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/ip-filter-policy</summary>
    IPolicyDocument IpFilter(PolicyExpression<string> action, Action<IIpFilterAddress> address);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-azure-ad-token-policy</summary>
    IPolicyDocument ValidateAzureAdToken(PolicyExpression<string> tenantIdOrUrl, PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, string? authenticationEndpoint = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateAzureAdTokenActions>? validationActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-client-certificate-policy</summary>
    IPolicyDocument ValidateClientCertificate(bool? validateRevocation = null, bool? validateTrust = null, bool? validateNotBefore = null, bool? validateNotAfter = null, bool? ignoreError = null, Action<IValidateClientCertificateIdentities>? identities = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-jwt-policy</summary>
    IPolicyDocument ValidateJwt(PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyExpression<bool>? requireExpirationTime = null, PolicyExpression<string>? requireScheme = null, PolicyExpression<bool>? requireSignedTokens = null, PolicyExpression<int>? clockSkewSeconds = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateJwtActions>? jwtActions = null);
}

public interface IIpFilterAddress
{
    IIpFilterAddress Add(params IEnumerable<PolicyExpression<string>> addresses);
    IIpFilterAddress AddRange(string fromAddress, string toAddress);
}

public interface ICheckHeaderValues
{
    ICheckHeaderValues Add(params IEnumerable<string> values);
}

public interface IValidateAzureAdTokenActions
{
    IValidateAzureAdTokenActions BackendApplicationIds(params IEnumerable<string> appIds);
    IValidateAzureAdTokenActions ClientApplicationIds(params IEnumerable<string> appIds);
    IValidateAzureAdTokenActions Audiences(params IEnumerable<string> audiences);
    IValidateAzureAdTokenActions RequiredClaims(Action<IValidateAzureRequiredClaims> claims);
    IValidateAzureAdTokenActions DecryptionKeys(params IEnumerable<string> certificateIds);
}
public interface IValidateAzureRequiredClaims
{
    IValidateAzureRequiredClaims Add(PolicyExpression<string> name, Action<IValidateAzureAdTokenClaimValues> values, PolicyExpression<string>? match = null, PolicyExpression<string>? separator = null);
}
public interface IValidateAzureAdTokenClaimValues
{
    IValidateAzureAdTokenClaimValues Add(PolicyExpression<string> value);
}

public interface IValidateClientCertificateIdentities
{
    IValidateClientCertificateIdentities Add(string? thumbprint = null, string? serialNumber = null, string? commonName = null, string? subject = null, string? dnsName = null, string? issuerSubject = null, string? issuerThumbprint = null, string? issuerCertificateId = null);
}

public interface IValidateJwtActions
{
    IValidateJwtActions OpenIdConfig(params IEnumerable<string> urls);
    IValidateJwtActions IssuerSigningKeys(Action<IValidateJwtIssuersSigningKeys> keys);
    IValidateJwtActions DecryptionKeys(Action<IValidateJwtDecryptionKeys> keys);
    IValidateJwtActions Audiences(params IEnumerable<string> audiences);
    IValidateJwtActions Issuers(params IEnumerable<string> issuers);
    IValidateJwtActions RequiredClaims(Action<IValidateAzureRequiredClaims> claims);
}

public interface IValidateJwtIssuersSigningKeys
{
    IValidateJwtIssuersSigningKeys Add(PolicyExpression<string>? keyBase64 = null, string? certificateId = null, string? id = null, string? n = null, string? e = null);
}

public interface IValidateJwtDecryptionKeys
{
    IValidateJwtDecryptionKeys Add(PolicyExpression<string>? keyBase64 = null, string? certificateId = null);
}

partial class PolicyDocument
{
    public IPolicyDocument AuthenticationBasic(PolicyExpression<string> username, PolicyExpression<string> password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.AuthenticationBasic(username, password);
        return this;
    }

    public IPolicyDocument AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body, PolicyExpression<string>? password)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        AuthenticationCertificateInternal(thumbprint, certificate, body, password);
        return this;
    }
    private IPolicyDocument AuthenticationCertificateInternal(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body, PolicyExpression<string>? password)
    {
        if (!String.IsNullOrEmpty(thumbprint) && !String.IsNullOrEmpty(certificate))
            throw new ArgumentException("Specify either a thumbprint or a certificate. Not both.", $"{nameof(thumbprint)}+{nameof(certificate)}");
        Writer.AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    public IPolicyDocument AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        // TODO: check variable exists
        Writer.AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }

    public IPolicyDocument CheckHeader(PolicyExpression<string> name, PolicyExpression<int> failedCheckHttpCode,
        PolicyExpression<string> failedCheckErrorMessage, PolicyExpression<bool> ignoreCase, Action<ICheckHeaderValues>? values = null)
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

        public ICheckHeaderValues Add(params IEnumerable<string> values)
        {
            foreach (var value in values)
                _writer.CheckHeaderValue(value);
            return this;
        }
    }

    public IPolicyDocument GetAuthorizationContext(PolicyExpression<string> providerId, PolicyExpression<string> authorizationId, PolicyVariable contextVariableName, PolicyExpression<string>? identity = null, PolicyExpression<bool>? ignoreError = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        Writer.GetAuthorizationContext(providerId, authorizationId, contextVariableName,
            identity is null ? "managed" : "jwt", identity, ignoreError);
        return this;
    }

    public IPolicyDocument IpFilter(PolicyExpression<string> action, Action<IIpFilterAddress> address)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Product | PolicyScopes.Api | PolicyScopes.Operation);
        Writer.IpFilter(action, () => address(new IpFilterAddress(Writer)));
        return this;
    }

    private sealed class IpFilterAddress : IIpFilterAddress
    {
        private readonly PolicyXmlWriter _writer;
        internal IpFilterAddress(PolicyXmlWriter writer) { _writer = writer; }

        public IIpFilterAddress Add(params IEnumerable<PolicyExpression<string>> addresses)
        {
            foreach (var address in addresses)
                _writer.IpFilterAddress(address);
            return this;
        }

        public IIpFilterAddress AddRange(string fromAddress, string toAddress)
        {
            _writer.IpFilterAddressRange(fromAddress, toAddress);
            return this;
        }
    }

    public IPolicyDocument ValidateAzureAdToken(PolicyExpression<string> tenantIdOrUrl, PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, string? authenticationEndpoint = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateAzureAdTokenActions>? validationActions = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        ThrowOnlyOne(headerName, queryParameterName, tokenValue);
        Action? writeNested = validationActions is null ? null : () => validationActions(new ValidateAzureAdTokenActions(Writer));
        Writer.ValidateAzureAdToken(tenantIdOrUrl, headerName, queryParameterName, tokenValue, authenticationEndpoint, failedValidationHttpCode, failedValidationErrorMessage, outputTokenVariableName, writeNested);
        return this;
    }

    private static void ThrowOnlyOne(string? headerName, string? queryParameterName, string? tokenValue)
    {
        if ((headerName is not null && (queryParameterName is not null || tokenValue is not null)) ||
            (queryParameterName is not null && (headerName is not null || tokenValue is not null)) ||
            (tokenValue is not null && (queryParameterName is not null || headerName is not null)))
            throw new ArgumentException($"Only one of {nameof(headerName)} or {nameof(queryParameterName)} or {nameof(tokenValue)} should be filled.", $"{nameof(headerName)}+{nameof(queryParameterName)}+{nameof(tokenValue)}");
    }

    private sealed class ValidateAzureAdTokenActions : IValidateAzureAdTokenActions, IValidateAzureRequiredClaims, IValidateAzureAdTokenClaimValues
    {
        private readonly PolicyXmlWriter _writer;
        public ValidateAzureAdTokenActions(PolicyXmlWriter writer) { _writer = writer; }

        public IValidateAzureAdTokenActions BackendApplicationIds(params IEnumerable<string> appIds)
        {
            _writer.ValidateAzureAdToken_BackendApplicationIds(appIds);
            return this;
        }

        public IValidateAzureAdTokenActions ClientApplicationIds(params IEnumerable<string> appIds)
        {
            _writer.ValidateAzureAdToken_ClientApplicationIds(appIds);
            return this;
        }

        public IValidateAzureAdTokenActions Audiences(params IEnumerable<string> audiences)
        {
            _writer.ValidateAzureAdToken_Audiences(audiences);
            return this;
        }

        public IValidateAzureAdTokenActions RequiredClaims(Action<IValidateAzureRequiredClaims> claims)
        {
            _writer.ValidateRequiredClaims(() => claims(this));
            return this;
        }

        public IValidateAzureAdTokenActions DecryptionKeys(params IEnumerable<string> certificateIds)
        {
            _writer.ValidateAzureAdToken_CertificateIds(certificateIds);
            return this;
        }

        public IValidateAzureRequiredClaims Add(PolicyExpression<string> name, Action<IValidateAzureAdTokenClaimValues> values, PolicyExpression<string>? match = null, PolicyExpression<string>? separator = null)
        {
            _writer.ValidateRequiredClaim(name, match, separator, () => values(this));
            return this;
        }

        public IValidateAzureAdTokenClaimValues Add(PolicyExpression<string> value)
        {
            _writer.ValidateRequiredClaimValue(value);
            return this;
        }
    }

    public IPolicyDocument ValidateClientCertificate(bool? validateRevocation = null, bool? validateTrust = null, bool? validateNotBefore = null, bool? validateNotAfter = null, bool? ignoreError = null, Action<IValidateClientCertificateIdentities>? identities = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? writeIdentities = identities is null ? null : () => identities(new ValidateClientCertificateIdentities(Writer));
        Writer.ValidateClientCertificate(validateRevocation, validateTrust, validateNotBefore, validateNotAfter, ignoreError, writeIdentities);
        return this;
    }

    private sealed class ValidateClientCertificateIdentities : IValidateClientCertificateIdentities
    {
        private readonly PolicyXmlWriter _writer;
        public ValidateClientCertificateIdentities(PolicyXmlWriter writer) { _writer = writer; }

        public IValidateClientCertificateIdentities Add(string? thumbprint = null, string? serialNumber = null, string? commonName = null, string? subject = null, string? dnsName = null, string? issuerSubject = null, string? issuerThumbprint = null, string? issuerCertificateId = null)
        {
            if (issuerCertificateId is not null && (issuerSubject is not null || issuerThumbprint is not null))
                throw new ArgumentException($"Specifying {nameof(issuerCertificateId)} is mutually exclusive with other issuer parameters.", $"{nameof(issuerSubject)}+{nameof(issuerThumbprint)}");

            _writer.ValidateClientCertificateIdentity(thumbprint, serialNumber, commonName, subject, dnsName, issuerSubject, issuerThumbprint, issuerCertificateId);
            return this;
        }
    }

    public IPolicyDocument ValidateJwt(PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyExpression<bool>? requireExpirationTime = null, PolicyExpression<string>? requireScheme = null, PolicyExpression<bool>? requireSignedTokens = null, PolicyExpression<int>? clockSkewSeconds = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateJwtActions>? jwtActions = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        ThrowOnlyOne(headerName, queryParameterName, tokenValue);
        Action? writeActions = jwtActions is null ? null : () => jwtActions(new ValidateJwtActions(Writer));
        Writer.ValidateJwt(headerName, queryParameterName, tokenValue, failedValidationHttpCode, failedValidationErrorMessage, requireExpirationTime, requireScheme, requireSignedTokens, clockSkewSeconds, outputTokenVariableName, writeActions);
        return this;
    }

    private sealed class ValidateJwtActions : IValidateJwtActions, IValidateJwtDecryptionKeys, IValidateJwtIssuersSigningKeys, IValidateAzureRequiredClaims, IValidateAzureAdTokenClaimValues
    {
        private readonly PolicyXmlWriter _writer;
        public ValidateJwtActions(PolicyXmlWriter writer) { _writer = writer; }

        public IValidateJwtActions OpenIdConfig(params IEnumerable<string> urls)
        {
            foreach (var url in urls)
                _writer.ValidateJwtOpenIdConfig(url);
            return this;
        }

        public IValidateJwtActions IssuerSigningKeys(Action<IValidateJwtIssuersSigningKeys> keys)
        {
            _writer.ValidateJwtIssuerSigningKeys(() => keys(this));
            return this;
        }

        public IValidateJwtActions DecryptionKeys(Action<IValidateJwtDecryptionKeys> keys)
        {
            _writer.ValidateJwtDecryptionKeys(() => keys(this));
            return this;
        }

        public IValidateJwtActions Audiences(params IEnumerable<string> audiences)
        {
            _writer.ValidateJwtAudiences(audiences);
            return this;
        }

        public IValidateJwtActions Issuers(params IEnumerable<string> issuers)
        {
            _writer.ValidateJwtIssuers(issuers);
            return this;
        }

        public IValidateJwtActions RequiredClaims(Action<IValidateAzureRequiredClaims> claims)
        {
            _writer.ValidateRequiredClaims(() => claims(this));
            return this;
        }

        public IValidateJwtDecryptionKeys Add(PolicyExpression<string>? keyBase64 = null, string? certificateId = null)
        {
            _writer.ValidateJwtDecryptionKey(keyBase64, certificateId);
            return this;
        }

        public IValidateJwtIssuersSigningKeys Add(PolicyExpression<string>? keyBase64 = null, string? certificateId = null, string? id = null, string? n = null, string? e = null)
        {
            _writer.ValidateJwtIssuerSigingKey(keyBase64, certificateId, id, n, e);
            return this;
        }

        public IValidateAzureRequiredClaims Add(PolicyExpression<string> name, Action<IValidateAzureAdTokenClaimValues> values, PolicyExpression<string>? match = null, PolicyExpression<string>? separator = null)
        {
            _writer.ValidateRequiredClaim(name, match, separator, () => values(this));
            return this;
        }

        public IValidateAzureAdTokenClaimValues Add(PolicyExpression<string> value)
        {
            _writer.ValidateRequiredClaimValue(value);
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

    public void GetAuthorizationContext(string providerId, string authorizationId, string contextVariableName,
        string? identityType, string? identity, string? ignoreError)
    {
        _xmlWriter.WriteStartElement("get-authorization-context");
        _xmlWriter.WriteAttributeStringOpt("provider-id", providerId);
        _xmlWriter.WriteAttributeStringOpt("authorization-id", authorizationId);
        _xmlWriter.WriteAttributeStringOpt("context-variable-name", contextVariableName);
        _xmlWriter.WriteAttributeStringOpt("identity-type", identityType);
        _xmlWriter.WriteAttributeStringOpt("identity", identity);
        _xmlWriter.WriteAttributeStringOpt("ignore-error", ignoreError);
        _xmlWriter.WriteEndElement();
    }

    public void IpFilter(string action, Action writeAddresses)
    {
        _xmlWriter.WriteStartElement("ip-filter");
        _xmlWriter.WriteAttributeStringOpt("action", action);
        _xmlWriter.WriteEndElement();
    }
    internal void IpFilterAddress(string address)
    {
        _xmlWriter.WriteElementString("address", address);
    }
    internal void IpFilterAddressRange(string from, string to)
    {
        _xmlWriter.WriteStartElement("address-range");
        _xmlWriter.WriteAttributeString("from", from);
        _xmlWriter.WriteAttributeString("to", to);
        _xmlWriter.WriteEndElement();
    }

    public void ValidateAzureAdToken(string tenantId, string? headerName, string? queryParameterName, string? tokenValue, string? authenticationEndpoint, string? failedValidationHttpCode, string? failedValidationErrorMessage, string? outputTokenVariableName, Action? writeNested)
    {
        _xmlWriter.WriteStartElement("validate-azure-ad-token");
        _xmlWriter.WriteAttributeString("tenant-id", tenantId);
        _xmlWriter.WriteAttributeStringOpt("header-name", headerName);
        _xmlWriter.WriteAttributeStringOpt("query-parameter-name", queryParameterName);
        _xmlWriter.WriteAttributeStringOpt("token-value", tokenValue);
        _xmlWriter.WriteAttributeStringOpt("authentication-endpoint", authenticationEndpoint);
        _xmlWriter.WriteAttributeStringOpt("failed-validation-httpcode", failedValidationHttpCode);
        _xmlWriter.WriteAttributeStringOpt("failed-validation-error-message", failedValidationErrorMessage);
        _xmlWriter.WriteAttributeStringOpt("output-token-variable-name", outputTokenVariableName);
        if (writeNested is not null) writeNested();
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateAzureAdToken_BackendApplicationIds(IEnumerable<string> appIds)
    {
        _xmlWriter.WriteStartElement("backend-application-ids");
        foreach (string appId in appIds)
            _xmlWriter.WriteElementString("application-id", appId);
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateAzureAdToken_ClientApplicationIds(IEnumerable<string> appIds)
    {
        _xmlWriter.WriteStartElement("client-application-ids");
        foreach (string appId in appIds)
            _xmlWriter.WriteElementString("application-id", appId);
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateAzureAdToken_Audiences(IEnumerable<string> audiences)
    {
        _xmlWriter.WriteStartElement("audiences");
        foreach (string audience in audiences)
            _xmlWriter.WriteElementString("audience", audience);
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateAzureAdToken_CertificateIds(IEnumerable<string> certificateIds)
    {
        _xmlWriter.WriteStartElement("decryption-keys");
        foreach (string certificateId in certificateIds)
        {
            _xmlWriter.WriteStartElement("key");
            _xmlWriter.WriteAttributeString("certificate-id", certificateId);
            _xmlWriter.WriteEndElement();
        }
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateRequiredClaims(Action writeClaims)
    {
        _xmlWriter.WriteStartElement("required-claims");
        writeClaims();
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateRequiredClaim(string name, string? match, string? separator, Action writeValues)
    {
        _xmlWriter.WriteStartElement("claim");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeStringOpt("match", match);
        _xmlWriter.WriteAttributeStringOpt("separator", separator);
        writeValues();
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateRequiredClaimValue(string value)
    {
        _xmlWriter.WriteElementString("value", value);
    }

    public void ValidateClientCertificate(bool? validateRevocation, bool? validateTrust, bool? validateNotBefore, bool? validateNotAfter, bool? ignoreError, Action? writeIdentities)
    {
        _xmlWriter.WriteStartElement("validate-client-certificate");
        _xmlWriter.WriteAttributeStringOpt("validate-revocation", BoolValue(validateRevocation));
        _xmlWriter.WriteAttributeStringOpt("validate-trust", BoolValue(validateTrust));
        _xmlWriter.WriteAttributeStringOpt("validate-not-before", BoolValue(validateNotBefore));
        _xmlWriter.WriteAttributeStringOpt("validate-not-after", BoolValue(validateNotAfter));
        _xmlWriter.WriteAttributeStringOpt("ignore-error", BoolValue(ignoreError));
        if (writeIdentities is not null)
        {
            _xmlWriter.WriteStartElement("identities");
            writeIdentities();
            _xmlWriter.WriteEndElement();
        }
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateClientCertificateIdentity(string? thumbprint, string? serialNumber, string? commonName, string? subject, string? dnsName, string? issuerSubject, string? issuerThumbprint, string? issuerCertificateId)
    {
        _xmlWriter.WriteStartElement("identity");
        _xmlWriter.WriteAttributeStringOpt("thumbprint", thumbprint);
        _xmlWriter.WriteAttributeStringOpt("serial-number", serialNumber);
        _xmlWriter.WriteAttributeStringOpt("common-name", commonName);
        _xmlWriter.WriteAttributeStringOpt("subject", subject);
        _xmlWriter.WriteAttributeStringOpt("dns-name", dnsName);
        _xmlWriter.WriteAttributeStringOpt("issuer-subject", issuerSubject);
        _xmlWriter.WriteAttributeStringOpt("issuer-thumbprint", issuerThumbprint);
        _xmlWriter.WriteAttributeStringOpt("issuer-certificate-id", issuerCertificateId);
        _xmlWriter.WriteEndElement();
    }

    public void ValidateJwt(string? headerName, string? queryParameterName, string? tokenValue, string? failedValidationHttpCode, string? failedValidationErrorMessage, string? requireExpirationTime, string? requireScheme, string? requireSignedTokens, string? clockSkew, string? outputTokenVariableName, Action? writeActions)
    {
        _xmlWriter.WriteStartElement("validate-jwt");
        _xmlWriter.WriteAttributeStringOpt("header-name", headerName);
        _xmlWriter.WriteAttributeStringOpt("query-parameter-name", queryParameterName);
        _xmlWriter.WriteAttributeStringOpt("token-value", tokenValue);
        _xmlWriter.WriteAttributeStringOpt("failed-validation-error-message", failedValidationErrorMessage);
        _xmlWriter.WriteAttributeStringOpt("failed-validation-httpcode", failedValidationHttpCode);
        _xmlWriter.WriteAttributeStringOpt("require-expiration-time", requireExpirationTime);
        _xmlWriter.WriteAttributeStringOpt("require-scheme", requireScheme);
        _xmlWriter.WriteAttributeStringOpt("require-signed-tokens", requireSignedTokens);
        _xmlWriter.WriteAttributeStringOpt("clock-skew", clockSkew);
        _xmlWriter.WriteAttributeStringOpt("output-token-variable-name", outputTokenVariableName);
        if (writeActions is not null)
        {
            writeActions();
        }
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateJwtOpenIdConfig(string url)
    {
        _xmlWriter.WriteStartElement("openid-config");
        _xmlWriter.WriteAttributeString("url", url);
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateJwtIssuerSigningKeys(Action keys)
    {
        _xmlWriter.WriteStartElement("issuer-signing-keys");
        keys();
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateJwtIssuerSigingKey(string? keyBase64, string? certificateId, string? id, string? n, string? e)
    {
        _xmlWriter.WriteStartElement("key");
        _xmlWriter.WriteAttributeStringOpt("certificate-id", certificateId);
        _xmlWriter.WriteAttributeStringOpt("id", id);
        _xmlWriter.WriteAttributeStringOpt("n", n);
        _xmlWriter.WriteAttributeStringOpt("e", e);
        if (keyBase64 is not null)
            _xmlWriter.WriteString(keyBase64);
        _xmlWriter.WriteEndElement();

    }
    internal void ValidateJwtDecryptionKeys(Action keys)
    {
        _xmlWriter.WriteStartElement("decryption-keys");
        keys();
        _xmlWriter.WriteEndElement();

    }
    internal void ValidateJwtDecryptionKey(string? keyBase64, string? certificateId)
    {
        _xmlWriter.WriteStartElement("key");
        _xmlWriter.WriteAttributeStringOpt("certificate-id", certificateId);
        if (keyBase64 is not null)
            _xmlWriter.WriteString(keyBase64);
        _xmlWriter.WriteEndElement();

    }
    internal void ValidateJwtAudiences(IEnumerable<string> audiences)
    {
        _xmlWriter.WriteStartElement("audiences");
        foreach (var audience in audiences)
            _xmlWriter.WriteElementString("audience", audience);
        _xmlWriter.WriteEndElement();

    }
    internal void ValidateJwtIssuers(IEnumerable<string> issuers)
    {
        _xmlWriter.WriteStartElement("issuers");
        foreach (var issuer in issuers)
            _xmlWriter.WriteElementString("issuer", issuer);
        _xmlWriter.WriteEndElement();
    }
}
