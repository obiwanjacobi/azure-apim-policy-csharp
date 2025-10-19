namespace AzureApimPolicyGen;

/// <summary>https://learn.microsoft.com/en-us/azure/api-management/api-management-policy-expressions</summary>
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
    public static PolicyExpression FromNamedValue(string name) => new($"{{{{{name}}}}}");
    public static PolicyExpression FromCode(string code)
    {
        CSharpCompiler.Verify(code);
        return new($"@({code})");
    }
    public static PolicyExpression FromCodeMultiline(string code)
    {
        CSharpCompiler.Verify(code);
        return new($"@{{{code}}}");
    }
}

