using System.Xml.Linq;
using Jacobi.Azure.ApiManagement.Policy;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class InboundPolicy : PolicyDocument
{
    protected override void Inbound()
    {
        this
            .AuthenticationBasic("username", "password")
            .CacheLookup(false, false,
                varyBy: varyBy => varyBy.Header("Content-Type").QueryParam("page"))
            .CheckHeader("Content-Type", 400, "Invalid media type", ignoreCase: true,
                values => values.Add("application/json"))
            .Choose(choose =>
                choose.When(PolicyExpression.FromCode("""Context.Variables.GetValueOrDefault<bool>("myvar", true)"""),
                    actions => actions.SetBody(LiquidTemplate.From(""" body """))))
            .Cors((cors) => cors
                    .AllowedOrigins(origin => origin.Any())
                    .AllowedMethods(methods => methods.Any())
                    .AllowedHeaders(headers => headers.Add("*")))
            .EmitMetric("metricName", null, "metricValue",
                dimensions => dimensions.Add("dim1", "val1").Add("dim2", "val2"))
            .GetAuthorizationContext("providerId", "authId", "authCtx")
            .IpFilter("allow", address => address.AddRange("10.0.0.0", "10.0.0.255"))
            .Proxy("http://hostname-or-ip:port", "username", "password")
            .PublishToDapr("Hello World", "new", "greetings", "responseVariable")
            .Quota(1000, 1024, 3600,
                api => api.Add(null, "api", 500, 1024, 1200,
                    operations => operations.Add(null, "operation", 500, 512, 1200)))
            .QuotaByKey(PolicyExpression.FromCode("Context.Request.IpAddress"), 1000, 4000, 3600,
                incrementCondition: PolicyExpression.FromCode("Context.Response.StatusCode >= 200 && Context.Response.StatusCode < 400"))
            .RateLimit(100, 100, "retryVar", "X-Retry-After", "remainingVar", "X-Remaining-Calls", "X-Total-Calls",
                apis => apis.Add(null, "myApi", 100, 100,
                    operations => operations.Add(null, "myOp", 10, 100)))
            .RateLimitByKey(PolicyExpression.FromCode("Context.Request.IpAddress"), 100, 60, null,
                PolicyExpression.FromCode("Context.Response.StatusCode == 200"),
                "retryVar", "X-Retry-After", "remainingVar", "X-Remaining-Calls", "X-Total-Calls")
            .RewriteUri("/api/v1", false)
            .SetBackendService("daprAppId", "daprMethod", "daprNamespace")
            .ValidateAzureAdToken("tenantId", "headerName", null, null, "authEndpoint", 401, "access denied", "outputVar",
                validations => validations
                    .BackendApplicationIds("backend1")
                    .ClientApplicationIds("client1")
                    .Audiences("audience")
                    .DecryptionKeys("cert")
                    .RequiredClaims(claims => claims.Add("claim", values => values.Add("admin"), "all"))
                )
            .ValidateClientCertificate(true, true, true, true, false,
                identities => identities.Add("thumbprint", "serialNumber", "commonName", "subject", "dnsName", "issuerSubject", "issuerThumbprint"))
            .ValidateJwt("headerName", null, null, 401, "access denied", true, "scheme", true, 100, "outputVar",
                actions => actions
                    .Audiences("audience")
                    .DecryptionKeys(keys => keys.Add("keyBase64", "certId"))
                    .Issuers("issuer")
                    .IssuerSigningKeys(keys => keys.Add("keyBase64", "certId", "id", "n", "e"))
                    .OpenIdConfig("openid")
                    .RequiredClaims(claims => claims.Add("claim", values => values.Add("admin"), "all")))
            .ValidateODataRequest("errorVar", "4.01", "3.0", "4.1", 2048)
            .ValidateParameters("ignore", "detect", "errorVar",
                actions => actions
                    .Headers("prevent", "detect", headers => headers.Add("headerName", "detect"))
                    .Path("detect", paths => paths.Add("path", "ignore"))
                    .Query("detect", "ignore", query => query.Add("query", "ignore")))

        ;

        base.Inbound();
    }
}

#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class InboundPolicyTest
{
    private readonly XDocument _document;

    public InboundPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.ToXDocument<InboundPolicy>();
        output.WriteLine(_document.ToPolicyXmlString());
    }

    [Fact]
    public void AuthenticationBasic()
    {
        var inbound = _document.Descendants("inbound").Single();
        var authBasic = inbound.Element("authentication-basic");
        Assert.NotNull(authBasic);
        Assert.Equal("username", authBasic.Attribute("username").Value);
        Assert.Equal("password", authBasic.Attribute("password").Value);
    }

    [Fact]
    public void CacheLookup()
    {
        var inbound = _document.Descendants("inbound").Single();
        var cacheLookup = inbound.Element("cache-lookup");
        Assert.NotNull(cacheLookup);
        Assert.Equal("false", cacheLookup.Attribute("vary-by-developer").Value);
        Assert.Equal("false", cacheLookup.Attribute("vary-by-developer-groups").Value);

        Assert.Equal("Content-Type", cacheLookup.Element("vary-by-header").Value);
        Assert.Equal("page", cacheLookup.Element("vary-by-query-parameter").Value);
    }

    [Fact]
    public void CheckHeader()
    {
        var inbound = _document.Descendants("inbound").Single();
        var checkHeader = inbound.Element("check-header");
        Assert.NotNull(checkHeader);
        Assert.Equal("Content-Type", checkHeader.Attribute("name").Value);
        Assert.Equal("400", checkHeader.Attribute("failed-check-http-code").Value);
        Assert.Equal("Invalid media type", checkHeader.Attribute("failed-check-error-message").Value);

        Assert.Equal("application/json", checkHeader.Element("value").Value);
    }

    [Fact]
    public void Choose()
    {
        var inbound = _document.Descendants("inbound").Single();
        var choose = inbound.Element("choose");
        Assert.NotNull(choose);
        var when = choose.Element("when");
        Assert.NotNull(when);
        Assert.NotNull(when.Element("set-body"));
    }

    [Fact]
    public void Cors()
    {
        var inbound = _document.Descendants("inbound").Single();
        var cors = inbound.Element("cors");
        Assert.NotNull(cors);
        var origins = cors.Element("allowed-origins");
        Assert.NotNull(origins);
        Assert.NotNull(origins.Element("origin"));

        var methods = cors.Element("allowed-methods");
        Assert.NotNull(methods);
        Assert.NotNull(methods.Element("method"));

        var headers = cors.Element("allowed-headers");
        Assert.NotNull(headers);
        Assert.NotNull(headers.Element("header"));
    }

    [Fact]
    public void EmitMetric()
    {
        var inbound = _document.Descendants("inbound").Single();
        var emitMetric = inbound.Element("emit-metric");
        Assert.NotNull(emitMetric);
        var dimensions = emitMetric.Elements("dimension");
        Assert.NotNull(dimensions);
        Assert.Equal("dim1", dimensions.ElementAt(0).Attribute("name").Value);
        Assert.Equal("dim2", dimensions.ElementAt(1).Attribute("name").Value);
    }

    [Fact]
    public void GetAuthorizationContext()
    {
        var inbound = _document.Descendants("inbound").Single();
        var authContext = inbound.Element("get-authorization-context");
        Assert.NotNull(authContext);
        Assert.Equal("providerId", authContext.Attribute("provider-id").Value);
        Assert.Equal("authId", authContext.Attribute("authorization-id").Value);
        Assert.Equal("authCtx", authContext.Attribute("context-variable-name").Value);
    }

    [Fact]
    public void Proxy()
    {
        var inbound = _document.Descendants("inbound").Single();
        var proxy = inbound.Element("proxy");
        Assert.NotNull(proxy);
        Assert.Equal("http://hostname-or-ip:port", proxy.Attribute("url").Value);
        Assert.Equal("username", proxy.Attribute("username").Value);
        Assert.Equal("password", proxy.Attribute("password").Value);
    }

    [Fact]
    public void PublishToDapr()
    {
        var inbound = _document.Descendants("inbound").Single();
        var publishToDapr = inbound.Element("publish-to-dapr");
        Assert.NotNull(publishToDapr);
        Assert.Equal("new", publishToDapr.Attribute("topic").Value);
        Assert.Equal("greetings", publishToDapr.Attribute("pubsub-name").Value);
        Assert.Equal("responseVariable", publishToDapr.Attribute("response-variable-name").Value);
        Assert.Equal("Hello World", publishToDapr.Value);
    }

    [Fact]
    public void Quota()
    {
        var inbound = _document.Descendants("inbound").Single();
        var quota = inbound.Element("quota");
        Assert.NotNull(quota);
        Assert.Equal("1000", quota.Attribute("calls").Value);
        Assert.Equal("1024", quota.Attribute("bandwidth").Value);
        Assert.Equal("3600", quota.Attribute("renewal-period").Value);
        var api = quota.Element("api");
        Assert.NotNull(api);
        Assert.Equal("api", api.Attribute("name").Value);
        Assert.Equal("500", api.Attribute("calls").Value);
        Assert.Equal("1024", api.Attribute("bandwidth").Value);
        Assert.Equal("1200", api.Attribute("renewal-period").Value);
        var operation = api.Element("operation");
        Assert.NotNull(operation);
        Assert.Equal("operation", operation.Attribute("name").Value);
        Assert.Equal("500", operation.Attribute("calls").Value);
        Assert.Equal("512", operation.Attribute("bandwidth").Value);
        Assert.Equal("1200", operation.Attribute("renewal-period").Value);
    }

    [Fact]
    public void QuotaByKey()
    {
        var inbound = _document.Descendants("inbound").Single();
        var quotaByKey = inbound.Element("quota-by-key");
        Assert.NotNull(quotaByKey);
        Assert.Equal("@(Context.Request.IpAddress)", quotaByKey.Attribute("counter-key").Value);
        Assert.Equal("1000", quotaByKey.Attribute("calls").Value);
        Assert.Equal("4000", quotaByKey.Attribute("bandwidth").Value);
        Assert.Equal("3600", quotaByKey.Attribute("renewal-period").Value);
        Assert.Equal("@(Context.Response.StatusCode >= 200 && Context.Response.StatusCode < 400)", quotaByKey.Attribute("increment-condition").Value);
    }

    [Fact]
    public void RateLimit()
    {
        var inbound = _document.Descendants("inbound").Single();
        var rateLimit = inbound.Element("rate-limit");
        Assert.NotNull(rateLimit);
        Assert.Equal("100", rateLimit.Attribute("calls").Value);
        Assert.Equal("100", rateLimit.Attribute("renewal-period").Value);
        Assert.Equal("retryVar", rateLimit.Attribute("retry-after-variable-name").Value);
        Assert.Equal("X-Retry-After", rateLimit.Attribute("retry-after-header-name").Value);
        Assert.Equal("remainingVar", rateLimit.Attribute("remaining-calls-variable-name").Value);
        Assert.Equal("X-Remaining-Calls", rateLimit.Attribute("remaining-calls-header-name").Value);
        Assert.Equal("X-Total-Calls", rateLimit.Attribute("total-calls-header-name").Value);
        var api = rateLimit.Element("api");
        Assert.NotNull(api);
        Assert.Equal("myApi", api.Attribute("name").Value);
        Assert.Equal("100", api.Attribute("calls").Value);
        Assert.Equal("100", api.Attribute("renewal-period").Value);
        var operation = api.Element("operation");
        Assert.NotNull(operation);
        Assert.Equal("myOp", operation.Attribute("name").Value);
        Assert.Equal("10", operation.Attribute("calls").Value);
        Assert.Equal("100", operation.Attribute("renewal-period").Value);
    }

    [Fact]
    public void RateLimitByKey()
    {
        var inbound = _document.Descendants("inbound").Single();
        var rateLimitByKey = inbound.Element("rate-limit-by-key");
        Assert.NotNull(rateLimitByKey);
        Assert.Equal("@(Context.Request.IpAddress)", rateLimitByKey.Attribute("counter-key").Value);
        Assert.Equal("100", rateLimitByKey.Attribute("calls").Value);
        Assert.Equal("60", rateLimitByKey.Attribute("renewal-period").Value);
        Assert.Equal("X-Retry-After", rateLimitByKey.Attribute("retry-after-header-name").Value);
        Assert.Equal("remainingVar", rateLimitByKey.Attribute("remaining-calls-variable-name").Value);
        Assert.Equal("X-Remaining-Calls", rateLimitByKey.Attribute("remaining-calls-header-name").Value);
        Assert.Equal("X-Total-Calls", rateLimitByKey.Attribute("total-calls-header-name").Value);
        Assert.Equal("@(Context.Response.StatusCode == 200)", rateLimitByKey.Attribute("increment-condition").Value);
    }

    [Fact]
    public void RewriteUri()
    {
        var inbound = _document.Descendants("inbound").Single();
        var rewriteUri = inbound.Element("rewrite-uri");
        Assert.NotNull(rewriteUri);
        Assert.Equal("/api/v1", rewriteUri.Attribute("template").Value);
        Assert.Equal("false", rewriteUri.Attribute("copy-unmatched-params").Value);
    }

    [Fact]
    public void SetBackendServiceDapr()
    {
        var inbound = _document.Descendants("inbound").Single();
        var setBackendService = inbound.Element("set-backend-service");
        Assert.NotNull(setBackendService);
        Assert.Equal("dapr", setBackendService.Attribute("backend-id").Value);
        Assert.Equal("daprAppId", setBackendService.Attribute("dapr-app-id").Value);
        Assert.Equal("daprMethod", setBackendService.Attribute("dapr-method").Value);
        Assert.Equal("daprNamespace", setBackendService.Attribute("dapr-namespace").Value);
    }

    [Fact]
    public void ValidateAzureAdToken()
    {
        var inbound = _document.Descendants("inbound").Single();
        var validateAzureAdToken = inbound.Element("validate-azure-ad-token");
        Assert.NotNull(validateAzureAdToken);
        Assert.Equal("tenantId", validateAzureAdToken.Attribute("tenant-id").Value);
        Assert.Equal("headerName", validateAzureAdToken.Attribute("header-name").Value);
        Assert.Equal("authEndpoint", validateAzureAdToken.Attribute("authentication-endpoint").Value);
        Assert.Equal("401", validateAzureAdToken.Attribute("failed-validation-httpcode").Value);
        Assert.Equal("access denied", validateAzureAdToken.Attribute("failed-validation-error-message").Value);
        Assert.Equal("outputVar", validateAzureAdToken.Attribute("output-token-variable-name").Value);
        var backendAppIds = validateAzureAdToken.Element("backend-application-ids");
        Assert.NotNull(backendAppIds);
        Assert.Equal("backend1", backendAppIds.Element("application-id").Value);
        var clientAppIds = validateAzureAdToken.Element("client-application-ids");
        Assert.NotNull(clientAppIds);
        Assert.Equal("client1", clientAppIds.Element("application-id").Value);
        var audiences = validateAzureAdToken.Element("audiences");
        Assert.NotNull(audiences);
        Assert.Equal("audience", audiences.Element("audience").Value);
        var decryptionKeys = validateAzureAdToken.Element("decryption-keys");
        Assert.NotNull(decryptionKeys);
        Assert.Equal("cert", decryptionKeys.Element("key").Attribute("certificate-id").Value);
        var requiredClaims = validateAzureAdToken.Element("required-claims");
        Assert.NotNull(requiredClaims);
        var claim = requiredClaims.Element("claim");
        Assert.Equal("claim", claim.Attribute("name").Value);
        Assert.Equal("all", claim.Attribute("match").Value);
        Assert.Equal("admin", claim.Value);
    }

    [Fact]
    public void ValidateClientCertificate()
    {
        var inbound = _document.Descendants("inbound").Single();
        var validateClientCertificate = inbound.Element("validate-client-certificate");
        Assert.NotNull(validateClientCertificate);
        Assert.Equal("true", validateClientCertificate.Attribute("validate-revocation").Value);
        Assert.Equal("true", validateClientCertificate.Attribute("validate-trust").Value);
        Assert.Equal("true", validateClientCertificate.Attribute("validate-not-before").Value);
        Assert.Equal("true", validateClientCertificate.Attribute("validate-not-after").Value);
        Assert.Equal("false", validateClientCertificate.Attribute("ignore-error").Value);
        var identities = validateClientCertificate.Element("identities");
        Assert.NotNull(identities);
        var identity = identities.Element("identity");
        Assert.NotNull(identity);
        Assert.Equal("thumbprint", identity.Attribute("thumbprint").Value);
        Assert.Equal("serialNumber", identity.Attribute("serial-number").Value);
        Assert.Equal("commonName", identity.Attribute("common-name").Value);
        Assert.Equal("subject", identity.Attribute("subject").Value);
        Assert.Equal("dnsName", identity.Attribute("dns-name").Value);
        Assert.Equal("issuerSubject", identity.Attribute("issuer-subject").Value);
        Assert.Equal("issuerThumbprint", identity.Attribute("issuer-thumbprint").Value);
    }

    [Fact]
    public void ValidateJwt()
    {
        var inbound = _document.Descendants("inbound").Single();
        var validateJwt = inbound.Element("validate-jwt");
        Assert.NotNull(validateJwt);
        Assert.Equal("headerName", validateJwt.Attribute("header-name").Value);
        Assert.Equal("401", validateJwt.Attribute("failed-validation-httpcode").Value);
        Assert.Equal("access denied", validateJwt.Attribute("failed-validation-error-message").Value);
        Assert.Equal("true", validateJwt.Attribute("require-expiration-time").Value);
        Assert.Equal("scheme", validateJwt.Attribute("require-scheme").Value);
        Assert.Equal("true", validateJwt.Attribute("require-signed-tokens").Value);
        Assert.Equal("100", validateJwt.Attribute("clock-skew").Value);
        Assert.Equal("outputVar", validateJwt.Attribute("output-token-variable-name").Value);
        var openIdConfig = validateJwt.Element("openid-config");
        Assert.NotNull(openIdConfig);
        Assert.Equal("openid", openIdConfig.Attribute("url").Value);
        var issuerSigningKeys = validateJwt.Element("issuer-signing-keys");
        Assert.NotNull(issuerSigningKeys);
        var key = issuerSigningKeys.Element("key");
        Assert.NotNull(key);
        Assert.Equal("keyBase64", key.Value);
        Assert.Equal("certId", key.Attribute("certificate-id").Value);
        Assert.Equal("id", key.Attribute("id").Value);
        Assert.Equal("n", key.Attribute("n").Value);
        Assert.Equal("e", key.Attribute("e").Value);
        var audiences = validateJwt.Element("audiences");
        Assert.NotNull(audiences);
        Assert.Equal("audience", audiences.Element("audience").Value);
        var decryptionKeys = validateJwt.Element("decryption-keys");
        Assert.NotNull(decryptionKeys);
        key = decryptionKeys.Element("key");
        Assert.NotNull(key);
        Assert.Equal("keyBase64", key.Value);
        Assert.Equal("certId", key.Attribute("certificate-id").Value);
        var requiredClaims = validateJwt.Element("required-claims");
        Assert.NotNull(requiredClaims);
        var claim = requiredClaims.Element("claim");
        Assert.Equal("claim", claim.Attribute("name").Value);
        Assert.Equal("all", claim.Attribute("match").Value);
        Assert.Equal("admin", claim.Value);
    }

    [Fact]
    public void ValidateODataRequest()
    {
        var inbound = _document.Descendants("inbound").Single();
        var validateODataRequest = inbound.Element("validate-odata-request");
        Assert.NotNull(validateODataRequest);
        Assert.Equal("errorVar", validateODataRequest.Attribute("error-variable-name").Value);
        Assert.Equal("4.01", validateODataRequest.Attribute("default-odata-version").Value);
        Assert.Equal("3.0", validateODataRequest.Attribute("min-odata-version").Value);
        Assert.Equal("4.1", validateODataRequest.Attribute("max-odata-version").Value);
        Assert.Equal("2048", validateODataRequest.Attribute("max-size").Value);
    }

    [Fact]
    public void ValidateParameters()
    {
        var inbound = _document.Descendants("inbound").Single();
        var validateParameters = inbound.Element("validate-parameters");
        Assert.NotNull(validateParameters);
        Assert.Equal("ignore", validateParameters.Attribute("specified-parameter-action").Value);
        Assert.Equal("detect", validateParameters.Attribute("unspecified-parameter-action").Value);
        Assert.Equal("errorVar", validateParameters.Attribute("error-variable-name").Value);
        var headers = validateParameters.Element("headers");
        Assert.NotNull(headers);
        Assert.Equal("prevent", headers.Attribute("specified-parameter-action").Value);
        Assert.Equal("detect", headers.Attribute("unspecified-parameter-action").Value);
        var parameter = headers.Element("parameter");
        Assert.Equal("headerName", parameter.Attribute("name").Value);
        Assert.Equal("detect", parameter.Attribute("action").Value);
        var query = validateParameters.Element("query");
        Assert.NotNull(query);
        Assert.Equal("detect", query.Attribute("specified-parameter-action").Value);
        Assert.Equal("ignore", query.Attribute("unspecified-parameter-action").Value);
        parameter = query.Element("parameter");
        Assert.Equal("query", parameter.Attribute("name").Value);
        Assert.Equal("ignore", parameter.Attribute("action").Value);
        var path = validateParameters.Element("path");
        Assert.NotNull(path);
        Assert.Equal("detect", path.Attribute("specified-parameter-action").Value);
        parameter = path.Element("parameter");
        Assert.Equal("path", parameter.Attribute("name").Value);
        Assert.Equal("ignore", parameter.Attribute("action").Value);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.