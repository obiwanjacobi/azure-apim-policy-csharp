using Jacobi.Azure.ApiManagement.Policy;

namespace AzureApimPolicy.Tests;

public class EmptyPolicy : PolicyDocument
{ }

public class EmptyPolicyTest
{
    [Fact]
    public void CheckXml()
    {
        var xDoc = PolicyXml.ToXDocument<EmptyPolicy>();

        var element = xDoc.Descendants("inbound").Single().Element("base");
        Assert.NotNull(element);
        element = xDoc.Descendants("backend").Single().Element("base");
        Assert.NotNull(element);
        element = xDoc.Descendants("outbound").Single().Element("base");
        Assert.NotNull(element);
        element = xDoc.Descendants("on-error").Single().Element("base");
        Assert.NotNull(element);
    }
}
