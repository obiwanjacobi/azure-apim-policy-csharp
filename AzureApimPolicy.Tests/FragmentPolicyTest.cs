using Jacobi.Azure.ApiManagement.Policy;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

public class FragmentPolicyTest
{
    private readonly ITestOutputHelper _output;

    public FragmentPolicyTest(ITestOutputHelper output)
    {
        _output = output;
    }

    internal sealed class WaitChooseWhenProxyFragment : PolicyFragment
    {
        protected override void Fragment(IPolicyFragment fragment)
        {
            fragment.Wait(actions => actions
                .Choose(choose => choose
                    .When(PolicyExpression.FromCode("true"), when => when.Proxy("https://localhost"))
                )
            );
        }
    }

    [Fact]
    public void WaitChooseWhenProxyFragmentTest()
    {
        var document = PolicyXml.FragmentToXDocument<WaitChooseWhenProxyFragment>();
        _output.WriteLine(document.ToPolicyXmlString());

        var fragment = document.Descendants("fragment").Single();
        var wait = fragment.Element("wait");
        Assert.NotNull(wait);
        var choose = wait.Element("choose");
        Assert.NotNull(choose);
        var when = choose.Element("when");
        Assert.NotNull(when);
        Assert.Equal("@(true)", when.Attribute("condition").Value);
        var proxy = when.Element("proxy");
        Assert.NotNull(proxy);
        Assert.Equal("https://localhost", proxy.Attribute("url").Value);
    }
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
