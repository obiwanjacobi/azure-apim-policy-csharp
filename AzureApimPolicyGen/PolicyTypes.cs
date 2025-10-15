namespace AzureApimPolicyGen;

public record struct PolicyVariable
{
    private string _name;

    public PolicyVariable(string name) => _name = name;

    public static implicit operator PolicyVariable(string name) => new(name);
    public static implicit operator string(PolicyVariable variable) => variable._name;
}

public record struct PolicyExpression
{
    private string _expression;

    public PolicyExpression(string expression) => _expression = expression;

    public static implicit operator PolicyExpression(bool value) => new(value ? "true" : "false");
    public static implicit operator PolicyExpression(string expression) => new(expression);
    public static implicit operator string(PolicyExpression expression) => expression._expression;
}