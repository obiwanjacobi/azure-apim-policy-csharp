namespace AzureApimPolicyGen;

/// <summary>https://learn.microsoft.com/en-us/azure/api-management/api-management-policy-expressions</summary>
public record struct PolicyExpression
{
    private readonly string? _code;
    private readonly CodeType _codeType;
    private readonly object? _value;

    private PolicyExpression(object? value)
    {
        _value = value;
    }

    private PolicyExpression(string? code, CodeType codeType)
    {
        _code = code;
        _codeType = codeType;
    }

    internal string? Code => _code;
    internal CodeType CodeType => _codeType;
    internal object? Value => _value;

    public override string ToString()
    {
        if (_code is not null) return _code;
        if (_value is not null) return _value.ToString() ?? String.Empty;

        return String.Empty;
    }

    public static implicit operator PolicyExpression(bool value) => From(value);
    public static implicit operator PolicyExpression(int value) => From(value);
    public static implicit operator PolicyExpression(string value) => new(value);

    public static PolicyExpression From(bool value) => new(value);
    public static PolicyExpression From(int value) => new(value);
    public static PolicyExpression ToBase64(byte[] bytes) => new(Convert.ToBase64String(bytes));
    public static PolicyExpression FromNamedValue(string name) => new($"{{{{{name}}}}}");
    public static PolicyExpression FromCode(string code)
    {
        if (code.Contains('\n'))
            throw new InvalidOperationException(
                $"Don't pass code with new-line characters to this method. Use the {nameof(FromCodeMultiline)} method instead.");

        return new(code, CodeType.SingleLine);
    }
    public static PolicyExpression FromCodeMultiline(string code)
    {
        return new(code, CodeType.Multiline);
    }
}

public record struct PolicyExpression<T>
{
    private readonly object _value;

    private PolicyExpression(object value)
    {
        _value = Convert.ChangeType(value, typeof(T));
    }

    public override string ToString()
    {
        if (typeof(T).FullName == typeof(bool).FullName)
            return (bool)_value ? "true" : "false";

        return _value.ToString() ?? String.Empty;
    }

    public static implicit operator PolicyExpression<T>(PolicyExpression expression)
    {
        var code = expression.Code;
        var val = expression.Value;

        if (code != null)
        {
            //var type = CSharpCompiler.Verify(code);
            CSharpCompiler.Verify(code);

            if (expression.CodeType == CodeType.SingleLine)
                return new($"@({code})");
            else
                return new($"@{{{code}}}");
        }
        if (val != null)
        {
            return new(val);
        }

        throw new InvalidOperationException();
    }

    public static implicit operator PolicyExpression<T>(T value) => new(value ?? throw new ArgumentNullException());
    public static implicit operator string(PolicyExpression<T> expression) => expression.ToString();
}

internal enum CodeType
{ SingleLine, Multiline }
