namespace AzureApimPolicyGen;

public interface ICheckHeaderValues
{
    ICheckHeaderValues Add(string value);
}

public interface IChooseActions
{
    IChooseActions When(PolicyExpression condition, Action<IPolicyDocument> whenActions);
    IChooseActions Otherwise(Action<IPolicyDocument> otherwiseActions);
}

public interface IEmitMetricDimensions
{
    IEmitMetricDimensions Add(string name, string? value);
}