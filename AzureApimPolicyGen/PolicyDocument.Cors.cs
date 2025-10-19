namespace AzureApimPolicyGen;


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
    ICorsAllowedOrigins Add(string origin);
}

public interface ICorsAllowedMethods
{
    ICorsAllowedMethods Any();
    ICorsAllowedMethods Add(HttpMethod method);
}

public interface ICorsAllowedHeaders
{
    ICorsAllowedHeaders Add(string header);
}

public interface ICorsExposedHeaders
{
    ICorsExposedHeaders Add(string header);
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

        ICorsAllowedOrigins ICorsAllowedOrigins.Add(string origin)
        {
            _writer.CorsAllowedOrigin(origin);
            return this;
        }

        ICorsAllowedMethods ICorsAllowedMethods.Any()
        {
            _writer.CorsAllowedMethod("*");
            return this;
        }

        ICorsAllowedMethods ICorsAllowedMethods.Add(HttpMethod method)
        {
            _writer.CorsAllowedMethod(method.ToString());
            return this;
        }

        ICorsAllowedHeaders ICorsAllowedHeaders.Add(string header)
        {
            _writer.CorsHeader(header);
            return this;
        }

        ICorsExposedHeaders ICorsExposedHeaders.Add(string header)
        {
            _writer.CorsHeader(header);
            return this;
        }
    }
}