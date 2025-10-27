using System.Reflection;
using System.Text.RegularExpressions;

namespace Jacobi.Azure.ApiManagement.Policy;

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
        using (var stream = File.OpenWrite(path))
        {
            policyDocument.WriteTo(stream);
        }

        var unescapedPath = path + ".unescaped";
        using (var streamUnescaped = File.OpenWrite(unescapedPath))
        {
            var xml = File.OpenText(path).ReadToEnd();
            XmlUnescape(xml, streamUnescaped);
        }

        File.Delete(path);
        File.Move(unescapedPath, path);
    }

    internal static void XmlUnescape(string xml, Stream streamUnescaped)
    {
        var regexSingle = new Regex(@"""@\((.*?)\)""", RegexOptions.Singleline);
        xml = regexSingle.Replace(xml, match =>
        {
            var code = match.Groups[1].Value;
            var unescaped = System.Net.WebUtility.HtmlDecode(code);
            return $"\"@({unescaped.Replace("Context.", "context.")})\"";
        });

        //var regexMulti = new Regex(@"@\{(.*?)\}");
        //xml = regexMulti.Replace(xml, match =>
        //{
        //    var code = match.Groups[1].Value;
        //    var unescaped = System.Net.WebUtility.HtmlDecode(code);
        //    return $"\"@{{{unescaped.Replace("Context.", "context.")})}}";
        //});

        using var writer = new StreamWriter(streamUnescaped, leaveOpen: true);
        writer.Write(xml);
    }
}

internal static class ReflectionExtensions
{
    public static bool IsPolicyDocument(this Type type)
        => type.IsAssignableTo(typeof(PolicyDocument));
}
