using System.Net.Mime;
using System.Xml.Linq;
using AzureApimPolicyGen;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class OutboundPolicy : PolicyDocument
{
    protected override void Outbound()
    {
        this
            .JsonToXml("always", considerAcceptHeader: false, parseDate: false,
                namespaceSeparator: ":", namespacePrefix: "xmlns", attributeBlockName: "#attrs")
            .MockResponse(200, MediaTypeNames.Application.Json)
            .RedirectContentUrls()
            .ReturnResponse(
                response => response
                    .SetHeader("Content-Type", "override", values => values.Add(MediaTypeNames.Application.Json))
                    .SetStatus(200, "OK")
                    .SetBody("""{ "data": 42 }""")
            )
        ;

        base.Outbound();
    }
}

#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class OutboundPolicyTest
{
    private readonly XDocument _document;

    public OutboundPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.ToXDocument<OutboundPolicy>();
        output.WriteLine(_document.ToPolicyXmlString());
    }

    [Fact]
    public void JsonToXml()
    {
        var outbound = _document.Descendants("outbound").Single();
        var jsonToXml = outbound.Element("json-to-xml");
        Assert.NotNull(jsonToXml);
        Assert.Equal("always", jsonToXml.Attribute("apply").Value);
        Assert.Equal("false", jsonToXml.Attribute("consider-accept-header").Value);
        Assert.Equal(":", jsonToXml.Attribute("namespace-separator").Value);
        Assert.Equal("xmlns", jsonToXml.Attribute("namespace-prefix").Value);
        Assert.Equal("#attrs", jsonToXml.Attribute("attribute-block-name").Value);
    }

    [Fact]
    public void MockResponse()
    {
        var outbound = _document.Descendants("outbound").Single();
        var mockResponse = outbound.Element("mock-response");
        Assert.NotNull(mockResponse);
        Assert.Equal("200", mockResponse.Attribute("status-code").Value);
        Assert.Equal("application/json", mockResponse.Attribute("content-type").Value);
    }

    [Fact]
    public void RedirectContentUrls()
    {
        var outbound = _document.Descendants("outbound").Single();
        var redirectContentUrls = outbound.Element("redirect-content-urls");
        Assert.NotNull(redirectContentUrls);
    }

    [Fact]
    public void ReturnResponse()
    {
        var outbound = _document.Descendants("outbound").Single();
        var returnResponse = outbound.Element("return-response");
        Assert.NotNull(returnResponse);
        var setHeader = returnResponse.Element("set-header");
        Assert.NotNull(setHeader);
        Assert.Equal("Content-Type", setHeader.Attribute("name").Value);
        Assert.Equal("override", setHeader.Attribute("exists-action").Value);
        Assert.Equal("application/json", setHeader.Value);
        var setStatus = returnResponse.Element("set-status");
        Assert.NotNull(setStatus);
        Assert.Equal("200", setStatus.Attribute("status-code").Value);
        Assert.Equal("OK", setStatus.Attribute("reason").Value);
        var setBody = returnResponse.Element("set-body");
        Assert.NotNull(setBody);
        Assert.Equal("""{ "data": 42 }""", setBody.Value);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
