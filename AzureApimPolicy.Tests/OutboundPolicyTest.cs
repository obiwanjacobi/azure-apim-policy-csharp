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
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
