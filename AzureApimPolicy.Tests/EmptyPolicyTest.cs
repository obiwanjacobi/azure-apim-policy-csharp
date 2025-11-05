using System.Xml.Linq;
using Jacobi.Azure.ApiManagement.Policy;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

public sealed class EmptyPolicy : PolicyDocument
{
    protected override void Inbound(IInbound inbound)
    {
        Comment("No inbound policies.");
        base.Inbound(inbound);
    }

    protected override void Backend(IBackend backend)
    {
        Comment("No backend policies.");
        base.Backend(backend);
    }
    protected override void Outbound(IOutbound outbound)
    {
        Comment("No outbound policies.");
        base.Outbound(outbound);
    }

    protected override void OnError(IOnError onError)
    {
        Comment("No error policies.");
        base.OnError(onError);
    }
}

public class EmptyPolicyTest
{
    private readonly XDocument _document;

    public EmptyPolicyTest(ITestOutputHelper output)
    {
        _document = PolicyXml.PolicyToXDocument<EmptyPolicy>();
        output.WriteLine(_document.ToPolicyXmlString());
    }

    [Fact]
    public void CheckXml()
    {
        var element = _document.Descendants("inbound").Single().Element("base");
        Assert.NotNull(element);
        element = _document.Descendants("backend").Single().Element("base");
        Assert.NotNull(element);
        element = _document.Descendants("outbound").Single().Element("base");
        Assert.NotNull(element);
        element = _document.Descendants("on-error").Single().Element("base");
        Assert.NotNull(element);
    }
}
