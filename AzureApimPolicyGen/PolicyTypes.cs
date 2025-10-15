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

    public static implicit operator PolicyExpression(bool value) => From(value);
    public static implicit operator PolicyExpression(int value) => From(value);
    public static implicit operator PolicyExpression(string expression) => new(expression);
    public static implicit operator string(PolicyExpression expression) => expression._expression;

    public static PolicyExpression From(bool value) => new(value ? "true" : "false");
    public static PolicyExpression From(int value) => new(value.ToString());
    public static PolicyExpression From(byte[] bytes) => Convert.ToBase64String(bytes);
}