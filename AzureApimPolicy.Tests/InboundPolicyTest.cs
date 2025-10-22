using System.Xml.Linq;
using AzureApimPolicyGen;
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
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.