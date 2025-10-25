using System.Xml.Linq;
using AzureApimPolicyGen;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

internal class OnErrorPolicy : PolicyDocument
{
    protected override void OnError()
    {
        this
            .LogToEventHub("loggerId", null, partitionKey: "partitionKey", message: "Error")
            .SetVariable("hasError", PolicyExpression.FromCode("true"))
        ;

        base.OnError();
    }
}

#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class OnErrorPolicyTest
{
    private readonly XDocument _document;

    public OnErrorPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.ToXDocument<OnErrorPolicy>();
        output.WriteLine(_document.ToPolicyXmlString());
    }

    [Fact]
    public void LogToEventHub()
    {
        var onError = _document.Descendants("on-error").Single();
        var logToEventHub = onError.Element("log-to-eventhub");
        Assert.NotNull(logToEventHub);
        Assert.Equal("loggerId", logToEventHub.Attribute("logger-id").Value);
        Assert.Equal("partitionKey", logToEventHub.Attribute("partition-key").Value);
        Assert.Equal("Error", logToEventHub.Value);
    }

    [Fact]
    public void SetVariable()
    {
        var onError = _document.Descendants("on-error").Single();
        var setVariable = onError.Element("set-variable");
        Assert.NotNull(setVariable);
        Assert.Equal("hasError", setVariable.Attribute("name").Value);
        Assert.Equal("@(true)", setVariable.Attribute("value").Value);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
