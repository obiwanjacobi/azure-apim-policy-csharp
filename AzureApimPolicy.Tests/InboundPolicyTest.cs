using System.Xml.Linq;
using AzureApimPolicyGen;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class InboundPolicy : PolicyDocument
{
    protected override void Inbound()
    {
        this
            .Authentication.Basic("username", "password")
            .Cache.Lookup(false, false, varyBy: varyBy =>
            {
                varyBy.Header("Content-Type");
                varyBy.QueryParam("page");
            })
            .CheckHeader("Content-Type", 400, "Invalid media type", true, (values) =>
            {
                values.Add("application/json");
            })
            .Choose(choose =>
            {
                choose.When(PolicyExpression.FromCode("""Context.Variables.GetValueOrDefault<bool>("myvar", true)"""),
                    actions =>
                    {
                        actions.SetBody(LiquidTemplate.From(""" """));
                    });
            })
            .Cors((cors) =>
            {
                cors
                    .AllowedOrigins(origin => origin.Any())
                    .AllowedMethods(methods => methods.Any())
                    .AllowedHeaders(headers => headers.Add("*"))
                    ;
            })
            .EmitMetric("metricName", null, "metricValue", (dimensions) =>
            {
                dimensions
                    .Add("dim1", "val1")
                    .Add("dim2", "val2");
            })
            ;

        base.Inbound();
    }
}

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
}
