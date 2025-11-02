using System.Xml.Linq;
using Jacobi.Azure.ApiManagement.Policy;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

public sealed class OnErrorPolicy : PolicyDocument
{
    protected override void OnError(IOnError onError)
    {
        onError
            .LogToEventHub("loggerId", null, partitionKey: "partitionKey", message: "Error")
            .SetVariable("hasError", PolicyExpression.FromCode("true"))
            .ValidateHeaders("detect", "prevent", "errorVar",
                headers => headers.Add("headerName", "detect"))
            .ValidateStatusCode("detect", "errorVar", codes => codes.Add(401, "ignore"))
        ;

        base.OnError(onError);
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

    [Fact]
    public void ValidateHeaders()
    {
        var onError = _document.Descendants("on-error").Single();
        var validateHeaders = onError.Element("validate-headers");
        Assert.NotNull(validateHeaders);
        Assert.Equal("detect", validateHeaders.Attribute("specified-header-action").Value);
        Assert.Equal("prevent", validateHeaders.Attribute("unspecified-header-action").Value);
        var header = validateHeaders.Element("header");
        Assert.NotNull(header);
        Assert.Equal("headerName", header.Attribute("name").Value);
        Assert.Equal("detect", header.Attribute("action").Value);
    }

    [Fact]
    public void ValidateStatusCode()
    {
        var onError = _document.Descendants("on-error").Single();
        var validateStatusCode = onError.Element("validate-status-code");
        Assert.NotNull(validateStatusCode);
        Assert.Equal("detect", validateStatusCode.Attribute("unspecified-status-code-action").Value);
        Assert.Equal("errorVar", validateStatusCode.Attribute("error-variable-name").Value);
        var statusCode = validateStatusCode.Element("status-code");
        Assert.NotNull(statusCode);
        Assert.Equal("401", statusCode.Attribute("code").Value);
        Assert.Equal("ignore", statusCode.Attribute("action").Value);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
