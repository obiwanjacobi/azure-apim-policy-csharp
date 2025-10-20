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
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
