namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#cross-domain

public interface ICrossDomain
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cors-policy</summary>
    IPolicyDocument Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cross-domain-policy</summary>
    /// <remarks>Xml Schema: https://www.adobe.com/xml/schemas/PolicyFile.xsd</remarks>
    IPolicyDocument CrossDomain(Action<ICrossDomainActions> actions, CrossDomainPolicies? permittedCrossDomainPolicies = null);
}

public interface ICorsActions
{
    ICorsActions AllowedOrigins(Action<ICorsAllowedOrigins> origins);
    ICorsActions AllowedMethods(Action<ICorsAllowedMethods> methods, int? preFlightResultMaxAge = null);
    ICorsActions AllowedHeaders(Action<ICorsAllowedHeaders> headers);
    ICorsActions ExposeHeaders(Action<ICorsExposedHeaders> headers);
}

public interface ICorsAllowedOrigins
{
    ICorsAllowedOrigins Any();
    // TODO: url templates?
    ICorsAllowedOrigins Add(params IEnumerable<string> origins);
}

public interface ICorsAllowedMethods
{
    ICorsAllowedMethods Any();
    ICorsAllowedMethods Add(params IEnumerable<HttpMethod> methods);
}

public interface ICorsAllowedHeaders
{
    ICorsAllowedHeaders Add(params IEnumerable<string> headers);
}

public interface ICorsExposedHeaders
{
    ICorsExposedHeaders Add(params IEnumerable<string> headers);
}

public interface ICrossDomainActions
{
    ICrossDomainActions AllowAccessFrom(string domain, string? toPorts = null, bool? secure = null);
    ICrossDomainActions AllowHttpRequestHeadersFrom(string domain, string headers, bool? secure = null);
    /// <summary>Can only be called once.</summary>
    ICrossDomainActions AllowAccessFromIdentity(string certificateFingerprint, string fingerprintAlgorithm);
}

public enum CrossDomainPolicies
{
    None, All, ByContentType, ByFtpFilename, MasterOnly
}

partial class PolicyDocument
{
    IInbound IInbound.Cors(Action<ICorsActions> actions, bool? allowCredentials, bool? terminateUnmatchedRequests)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.Cors(() => actions(new CorsActions(Writer)), allowCredentials, terminateUnmatchedRequests);
        return this;
    }

    private sealed class CorsActions : ICorsActions,
        ICorsAllowedOrigins, ICorsAllowedMethods, ICorsAllowedHeaders, ICorsExposedHeaders
    {
        private readonly PolicyXmlWriter _writer;
        internal CorsActions(PolicyXmlWriter writer) => _writer = writer;

        public ICorsActions AllowedOrigins(Action<ICorsAllowedOrigins> origins)
        {
            _writer.CorsAllowedOrigins(() => origins(this));
            return this;
        }

        public ICorsActions AllowedMethods(Action<ICorsAllowedMethods> methods, int? preFlightResultMaxAge)
        {
            _writer.CorsAllowedMethods(() => methods(this), preFlightResultMaxAge.ToString());
            return this;
        }

        public ICorsActions AllowedHeaders(Action<ICorsAllowedHeaders> headers)
        {
            _writer.CorsAllowedHeaders(() => headers(this));
            return this;
        }

        public ICorsActions ExposeHeaders(Action<ICorsExposedHeaders> headers)
        {
            _writer.CorsExposedHeaders(() => headers(this));
            return this;
        }

        ICorsAllowedOrigins ICorsAllowedOrigins.Any()
        {
            _writer.CorsAllowedOrigin("*");
            return this;
        }

        ICorsAllowedOrigins ICorsAllowedOrigins.Add(params IEnumerable<string> origins)
        {
            foreach (var origin in origins)
                _writer.CorsAllowedOrigin(origin);
            return this;
        }

        ICorsAllowedMethods ICorsAllowedMethods.Any()
        {
            _writer.CorsAllowedMethod("*");
            return this;
        }

        ICorsAllowedMethods ICorsAllowedMethods.Add(params IEnumerable<HttpMethod> methods)
        {
            foreach (var method in methods)
                _writer.CorsAllowedMethod(method.ToString());
            return this;
        }

        ICorsAllowedHeaders ICorsAllowedHeaders.Add(params IEnumerable<string> headers)
        {
            foreach (var header in headers)
                _writer.CorsHeader(header);
            return this;
        }

        ICorsExposedHeaders ICorsExposedHeaders.Add(params IEnumerable<string> headers)
        {
            foreach (var header in headers)
                _writer.CorsHeader(header);
            return this;
        }
    }

    IInbound IInbound.CrossDomain(Action<ICrossDomainActions> actions, CrossDomainPolicies? permittedCrossDomainPolicies)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global);
        Writer.CrossDomain(() => actions(new CrossDomainActions(Writer)), CrossDomainPoliciesToString(permittedCrossDomainPolicies));
        return this;

        static string? CrossDomainPoliciesToString(CrossDomainPolicies? crossDomainPolicies)
            => crossDomainPolicies switch
            {
                CrossDomainPolicies.None => "none",
                CrossDomainPolicies.All => "all",
                CrossDomainPolicies.ByContentType => "by-content-type",
                CrossDomainPolicies.ByFtpFilename => "by-ftp-filename",
                CrossDomainPolicies.MasterOnly => "master-only",
                _ => null
            };
    }

    private sealed class CrossDomainActions : ICrossDomainActions
    {
        private readonly PolicyXmlWriter _writer;
        public CrossDomainActions(PolicyXmlWriter writer) { _writer = writer; }

        public ICrossDomainActions AllowAccessFrom(string domain, string? toPorts = null, bool? secure = null)
        {
            _writer.CrossDomainAllowAccessFrom(domain, toPorts, secure);
            return this;
        }

        public ICrossDomainActions AllowHttpRequestHeadersFrom(string domain, string headers, bool? secure = null)
        {
            _writer.CrossDomainAllowHttpRequestHeadersFrom(domain, headers, secure);
            return this;
        }

        public ICrossDomainActions AllowAccessFromIdentity(string certificateFingerprint, string fingerprintAlgorithm)
        {
            _writer.CrossDomainAllowAccessFromIdentity(certificateFingerprint, fingerprintAlgorithm);
            return this;
        }
    }
}

partial class PolicyXmlWriter
{
    public void Cors(Action writeValues, bool? allowCredentials, bool? terminateUnmatchedRequests)
    {
        _xmlWriter.WriteStartElement("cors");
        _xmlWriter.WriteAttributeStringOpt("allow-credentials", BoolValue(allowCredentials));
        _xmlWriter.WriteAttributeStringOpt("terminate-unmatched-request", BoolValue(terminateUnmatchedRequests));
        writeValues();
        _xmlWriter.WriteEndElement();
    }
    internal void CorsAllowedOrigins(Action origins)
    {
        _xmlWriter.WriteStartElement("allowed-origins");
        origins();
        _xmlWriter.WriteEndElement();
    }
    internal void CorsAllowedOrigin(string origin)
    {
        _xmlWriter.WriteElementString("origin", origin);
    }
    internal void CorsAllowedMethods(Action methods, string? preflightResultMaxAge)
    {
        _xmlWriter.WriteStartElement("allowed-methods");
        _xmlWriter.WriteAttributeStringOpt("pre-flight-max-age", preflightResultMaxAge);
        methods();
        _xmlWriter.WriteEndElement();
    }
    internal void CorsAllowedMethod(string method)
    {
        _xmlWriter.WriteElementString("method", method);
    }
    internal void CorsAllowedHeaders(Action headers)
    {
        _xmlWriter.WriteStartElement("allowed-headers");
        headers();
        _xmlWriter.WriteEndElement();
    }
    internal void CorsExposedHeaders(Action headers)
    {
        _xmlWriter.WriteStartElement("exposed-headers");
        headers();
        _xmlWriter.WriteEndElement();
    }
    internal void CorsHeader(string header)
    {
        _xmlWriter.WriteElementString("header", header);
    }

    public void CrossDomain(Action writeActions, string? permittedCrossDomainPolicies)
    {
        _xmlWriter.WriteStartElement("cross-domain-policy");
        if (permittedCrossDomainPolicies is not null)
        {
            _xmlWriter.WriteStartElement("site-control");
            _xmlWriter.WriteAttributeString("permitted-cross-domain-policies", permittedCrossDomainPolicies);
            _xmlWriter.WriteEndElement();
        }
        writeActions();
        _xmlWriter.WriteEndElement();
    }
    internal void CrossDomainAllowAccessFrom(string domain, string? toPorts, bool? secure)
    {
        _xmlWriter.WriteStartElement("allow-access-from");
        _xmlWriter.WriteAttributeString("domain", domain);
        _xmlWriter.WriteAttributeStringOpt("to-ports", toPorts);
        _xmlWriter.WriteAttributeStringOpt("secure", BoolValue(secure));
        _xmlWriter.WriteEndElement();
    }
    internal void CrossDomainAllowHttpRequestHeadersFrom(string domain, string headers, bool? secure)
    {
        _xmlWriter.WriteStartElement("allow-http-request-headers-from");
        _xmlWriter.WriteAttributeString("domain", domain);
        _xmlWriter.WriteAttributeStringOpt("headers", headers);
        _xmlWriter.WriteAttributeStringOpt("secure", BoolValue(secure));
        _xmlWriter.WriteEndElement();
    }
    internal void CrossDomainAllowAccessFromIdentity(string certificateFingerprint, string fingerprintAlgorithm)
    {
        _xmlWriter.WriteStartElement("allow-access-from-identity");
        _xmlWriter.WriteStartElement("signatory");
        _xmlWriter.WriteStartElement("certificate");
        _xmlWriter.WriteAttributeString("fingerprint", certificateFingerprint);
        _xmlWriter.WriteAttributeStringOpt("fingerprint-algorithm", fingerprintAlgorithm);
        _xmlWriter.WriteEndElement();
        _xmlWriter.WriteEndElement();
        _xmlWriter.WriteEndElement();
    }
}