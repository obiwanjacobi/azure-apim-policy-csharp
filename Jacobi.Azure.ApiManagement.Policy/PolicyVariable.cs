namespace Jacobi.Azure.ApiManagement.Policy;

public record struct PolicyVariable
{
    private string _name;

    public PolicyVariable(string name) => _name = name;

    public static implicit operator PolicyVariable(string name) => new(name);
    public static implicit operator string(PolicyVariable variable) => variable._name;
}
