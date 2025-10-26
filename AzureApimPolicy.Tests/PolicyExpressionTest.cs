using AzureApimPolicyGen;

namespace AzureApimPolicy.Tests;

public class PolicyExpressionTest
{
    [Fact]
    public void BoolExpression()
    {
        PolicyExpression expr = true;
        Assert.Equal(true.ToString(), expr.ToString());

        PolicyExpression<bool> polStr = expr;
        Assert.Equal("true", polStr.ToString());

        polStr = true;
        Assert.Equal("true", polStr.ToString());
    }

    [Fact]
    public void Int32Expression()
    {
        PolicyExpression expr = 42;
        Assert.Equal(42.ToString(), expr.ToString());

        PolicyExpression<int> polStr = expr;
        Assert.Equal("42", polStr.ToString());

        polStr = 42;
        Assert.Equal("42", polStr.ToString());
    }

    [Fact]
    public void StringExpression()
    {
        PolicyExpression expr = "Hello World";
        Assert.Equal("Hello World", expr.ToString());

        PolicyExpression<string> polStr = expr;
        Assert.Equal(polStr.ToString(), expr.ToString());
    }

    [Fact]
    public void StringEnumExpression()
    {

        PolicyExpression expr = "Hello World";
        Assert.Equal("Hello World", expr.ToString());

        PolicyExpression<string> polStr = expr;
        Assert.Equal(polStr.ToString(), expr.ToString());
    }
}
