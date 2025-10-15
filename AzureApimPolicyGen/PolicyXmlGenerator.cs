using System.Reflection;

namespace AzureApimPolicyGen;

public sealed class PolicyXmlGenerator
{
    private readonly string _basePath;

    public PolicyXmlGenerator()
        => _basePath = ".\\";

    public PolicyXmlGenerator(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public void GenerateAll(string assemblyPath)
    {
        var assembly = Assembly.Load(assemblyPath);
        GenerateAll(assembly);
    }

    public void GenerateAll(Assembly assembly)
    {
        var policyDocuments = GatherPolicyDocuments(assembly);
        GenerateXml(policyDocuments);
    }

    public List<PolicyDocument> GatherPolicyDocuments(Assembly assembly)
        => assembly.GetTypes()
            .Where(t => t.IsPolicyDocument())
            .Select(t => (PolicyDocument?)Activator.CreateInstance(t) ??
                throw new Exception($"Could not instantiate PolicyDocument: {t.Name}."))
            .ToList();


    public void GenerateXml(List<PolicyDocument> policyDocuments)
    {
        foreach (var policyDocument in policyDocuments)
        {
            GenerateXml(policyDocument);
        }
    }

    public void GenerateXml(PolicyDocument policyDocument)
    {
        var path = Path.Combine(_basePath, $"{policyDocument.GetType().Name}.xml");
        policyDocument.WriteTo(path);
    }
}


internal static class ReflectionExtensions
{
    public static bool IsPolicyDocument(this Type type)
        => type.IsAssignableTo(typeof(PolicyDocument));

}