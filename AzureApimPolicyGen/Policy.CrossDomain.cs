namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#cross-domain

public interface ICrossDomain
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cors-policy</summary>
    IPolicyDocument Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);
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

partial class PolicyDocument
{
    public IPolicyDocument Cors(Action<ICorsActions> actions, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null)
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
}