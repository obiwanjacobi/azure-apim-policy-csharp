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
            .SendServiceBusMessage("sb-test", "Hello World!", props => props.Add("correlation-id", "42"), null, "myTopic")
            .Trace("Test", "Trace Test", TraceSeverity.Verbose, "traceid", "42")
            .ValidateContent("detect", 1024, "ignore", "errorVar",
                validation => validation
                    .ContentTypeMap("anyValue", "missingValue", types => types.Add("from", "to"))
                    .Content(ValidateContentAs.Json, "type", "schemaId", "schemaRef", false, true))
            .XmlToJson("direct", "always", true, true)
            .XslTransform("""<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"></xsl:stylesheet>""",
                parameters => parameters.Add("param", "42"))
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

    [Fact]
    public void SendServiceBusMessage()
    {
        var outbound = _document.Descendants("outbound").Single();
        var sendServiceBusMessage = outbound.Element("send-service-bus-message");
        Assert.NotNull(sendServiceBusMessage);
        Assert.Equal("sb-test", sendServiceBusMessage.Attribute("namespace").Value);
        Assert.Equal("myTopic", sendServiceBusMessage.Attribute("topic-name").Value);
        var messageProperties = sendServiceBusMessage.Element("message-properties");
        Assert.NotNull(messageProperties);
        var messageProperty = messageProperties.Element("message-property");
        Assert.NotNull(messageProperty);
        Assert.Equal("correlation-id", messageProperty.Attribute("name").Value);
        Assert.Equal("42", messageProperties.Value);
        var payload = sendServiceBusMessage.Element("payload");
        Assert.NotNull(payload);
        Assert.Equal("Hello World!", payload.Value);
    }

    [Fact]
    public void Trace()
    {
        var outbound = _document.Descendants("outbound").Single();
        var trace = outbound.Element("trace");
        Assert.NotNull(trace);
        Assert.Equal("Test", trace.Attribute("source").Value);
        Assert.Equal("verbose", trace.Attribute("severity").Value);
        var message = trace.Element("message");
        Assert.NotNull(message);
        Assert.Equal("Trace Test", message.Value);
        var metadata = trace.Element("metadata");
        Assert.NotNull(metadata);
        Assert.Equal("traceid", metadata.Attribute("name").Value);
        Assert.Equal("42", metadata.Attribute("value").Value);
    }

    [Fact]
    public void ValidateContent()
    {
        var outbound = _document.Descendants("outbound").Single();
        var validateContent = outbound.Element("validate-content");
        Assert.NotNull(validateContent);
        Assert.Equal("detect", validateContent.Attribute("unspecified-content-type-action").Value);
        Assert.Equal("1024", validateContent.Attribute("max-size").Value);
        Assert.Equal("ignore", validateContent.Attribute("size-exceed-action").Value);
        Assert.Equal("errorVar", validateContent.Attribute("errors-variable-name").Value);
        var contentTypeMap = validateContent.Element("content-type-map");
        Assert.NotNull(contentTypeMap);
        Assert.Equal("anyValue", contentTypeMap.Attribute("any-content-type-value").Value);
        Assert.Equal("missingValue", contentTypeMap.Attribute("missing-content-type-value").Value);
        var type = contentTypeMap.Element("type");
        Assert.NotNull(type);
        Assert.Equal("from", type.Attribute("from").Value);
        Assert.Equal("to", type.Attribute("to").Value);
        var content = validateContent.Element("content");
        Assert.NotNull(content);
        Assert.Equal("json", content.Attribute("validate-as").Value);
        Assert.Equal("type", content.Attribute("type").Value);
        Assert.Equal("schemaId", content.Attribute("schema-id").Value);
        Assert.Equal("schemaRef", content.Attribute("schema-ref").Value);
        Assert.Equal("false", content.Attribute("allow-additional-properties").Value);
        Assert.Equal("true", content.Attribute("case-insensitive-property-names").Value);
    }

    [Fact]
    public void XmlToJson()
    {
        var outbound = _document.Descendants("outbound").Single();
        var xmlToJson = outbound.Element("xml-to-json");
        Assert.NotNull(xmlToJson);
        Assert.Equal("direct", xmlToJson.Attribute("kind").Value);
        Assert.Equal("always", xmlToJson.Attribute("apply").Value);
        Assert.Equal("true", xmlToJson.Attribute("consider-accept-header").Value);
        Assert.Equal("true", xmlToJson.Attribute("always-array-child-elements").Value);
    }

    [Fact]
    public void XslTransform()
    {
        var outbound = _document.Descendants("outbound").Single();
        var xslTransform = outbound.Element("xsl-transform");
        Assert.NotNull(xslTransform);
        var param = xslTransform.Element("parameter");
        Assert.NotNull(param);
        Assert.Equal("param", param.Attribute("parameter-name").Value);
        Assert.Equal("42", param.Value);
        // Cannot get the correct code to detect the xslt sheet
        // but you can verify it in the output
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
