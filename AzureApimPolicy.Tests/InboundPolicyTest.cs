using System.Xml.Linq;
using AzureApimPolicyGen;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class InboundPolicy : PolicyDocument
{
    protected override void Inbound()
    {
        Authentication.Basic("username", "password")
            .Cache.Lookup(false, false, varyBy: varyBy =>
            {
                varyBy.Header("Content-Type");
                varyBy.QueryParam("page");
            })
            .CheckHeader("Content-Type", 400, "Invalid media type", true, (values) =>
            {
                values.Add("application/json");
            });

        base.Inbound();
    }
}

public class InboundyPolicyTest
{
    private readonly XDocument _document;

    public InboundyPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.ToXDocument<InboundPolicy>();
        output.WriteLine(_document.ToString());
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
}
