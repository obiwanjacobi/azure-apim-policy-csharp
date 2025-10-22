using System.Net.Mime;
using System.Xml.Linq;
using AzureApimPolicyGen;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class BackendPolicy : PolicyDocument
{
    protected override void Backend()
    {
        this
            .ForwardRequest(timeoutSeconds: 30)
            .IncludeFragment("myFragment")
            .LimitConcurrency(PolicyExpression.FromCode("""(string)Context.Variables["connectionId"]"""), 120,
                actions => actions.ForwardRequest(timeoutSeconds: 120))
            .Retry(PolicyExpression.FromCode("true"), 3, 10, 30, firstFastRetry: true)
            .SendOneWayRequest(request => request
                    .SetUrl("https://localhost/post")
                    .SetMethod("POST")
                    .SetHeader("Content-Type", "override", values => values.Add(MediaTypeNames.Application.Json))
                    .SetBody("""{ "value": "42" }"""),
                "copy", 120)
            ;

        base.Backend();
    }
}

#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class BackendPolicyTest
{
    private readonly XDocument _document;

    public BackendPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.ToXDocument<BackendPolicy>();
        output.WriteLine(_document.ToPolicyXmlString());
    }

    [Fact]
    public void ForwardRequest()
    {
        var backend = _document.Descendants("backend").Single();
        var forwardRequest = backend.Element("forward-request");
        Assert.NotNull(forwardRequest);
        Assert.Equal("30", forwardRequest.Attribute("timeout").Value);
    }

    [Fact]
    public void IncludeFragment()
    {
        var backend = _document.Descendants("backend").Single();
        var includeFragment = backend.Element("include-fragment");
        Assert.NotNull(includeFragment);
        Assert.Equal("myFragment", includeFragment.Attribute("fragment-id").Value);
    }

    [Fact]
    public void LimitConcurrency()
    {
        var backend = _document.Descendants("backend").Single();
        var limitConcurrency = backend.Element("limit-concurrency");
        Assert.NotNull(limitConcurrency);
        Assert.Equal("""@((string)Context.Variables["connectionId"])""", limitConcurrency.Attribute("key").Value);
        Assert.Equal("120", limitConcurrency.Attribute("max-count").Value);
        var forwardRequest = limitConcurrency.Element("forward-request");
        Assert.NotNull(forwardRequest);
    }

    [Fact]
    public void Retry()
    {
        var backend = _document.Descendants("backend").Single();
        var retry = backend.Element("retry");
        Assert.NotNull(retry);
        Assert.Equal("""@(true)""", retry.Attribute("condition").Value);
        Assert.Equal("3", retry.Attribute("count").Value);
        Assert.Equal("10", retry.Attribute("interval").Value);
        Assert.Equal("30", retry.Attribute("max-interval").Value);
        Assert.Equal("true", retry.Attribute("first-fast-retry").Value);
    }

    [Fact]
    public void SendOneWayRequest()
    {
        var backend = _document.Descendants("backend").Single();
        var sendOneWayRequest = backend.Element("send-one-way-request");
        Assert.NotNull(sendOneWayRequest);
        Assert.Equal("copy", sendOneWayRequest.Attribute("mode").Value);
        Assert.Equal("120", sendOneWayRequest.Attribute("timeout").Value);
        var setUrl = sendOneWayRequest.Element("set-url");
        Assert.NotNull(setUrl);
        Assert.Equal("https://localhost/post", setUrl.Value);
        var setMethod = sendOneWayRequest.Element("set-method");
        Assert.NotNull(setMethod);
        Assert.Equal("POST", setMethod.Value);
        var setHeader = sendOneWayRequest.Element("set-header");
        Assert.NotNull(setHeader);
        Assert.Equal("Content-Type", setHeader.Attribute("name").Value);
        Assert.Equal("override", setHeader.Attribute("exists-action").Value);
        var setHeaderValue = setHeader.Element("value");
        Assert.NotNull(setHeaderValue);
        Assert.Equal("application/json", setHeaderValue.Value);
        var setBody = sendOneWayRequest.Element("set-body");
        Assert.NotNull(setBody);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
