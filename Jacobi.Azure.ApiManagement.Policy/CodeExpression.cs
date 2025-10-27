using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jacobi.Azure.ApiManagement.Policy;

internal sealed class CSharpCompiler
{
    private static readonly List<MetadataReference> _references = PolicyExpressionDependencies.GetMetadataReferences();
    private static readonly string _usings = PolicyExpressionDependencies.GetUsings();

    public static void Verify(string code)
    {
        var codeTemplate = WrapCode(code);
        var syntaxTree = CSharpSyntaxTree.ParseText(codeTemplate);
        var diagnostics = syntaxTree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        if (diagnostics.Any())
            throw new Exception("Syntax Error(s) at " + String.Join(Environment.NewLine, diagnostics));

        var compilation = CSharpCompilation.Create(
            "PolicyExpressionCheck",
            [syntaxTree],
            _references,
            new CSharpCompilationOptions(OutputKind.WindowsApplication)
        );

        diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        // CS0201 statement expression error - happens a lot with the code snippets
        if (diagnostics.Where(d => d.Id != "CS0201").Any())
            throw new Exception("Compilation Error(s) at " + String.Join(Environment.NewLine, diagnostics));

        //var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
        //semanticModel.LookupNamespacesAndTypes()
    }

    private static string WrapCode(string code)
    {
        return $"""
            {_usings}

            using Jacobi.Azure.ApiManagement.Policy;

            {code};
            """;
    }
}

internal static class PolicyExpressionDependencies
{
    // https://learn.microsoft.com/en-us/azure/api-management/api-management-policy-expressions#CLRTypes
    private static readonly string[] SupportedTypes = new[]
{
    // Newtonsoft.Json
    "Newtonsoft.Json.Formatting, Newtonsoft.Json",
    "Newtonsoft.Json.JsonConvert, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.Extensions, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JArray, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JConstructor, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JContainer, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JObject, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JProperty, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JRaw, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JToken, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JTokenType, Newtonsoft.Json",
    "Newtonsoft.Json.Linq.JValue, Newtonsoft.Json",
    // System.*
    "System.Array, System.Private.CoreLib",
    "System.BitConverter, System.Private.CoreLib",
    "System.Boolean, System.Private.CoreLib",
    "System.Byte, System.Private.CoreLib",
    "System.Char, System.Private.CoreLib",
    "System.Collections.Generic.Dictionary`2, System.Collections",
    "System.Collections.Generic.HashSet`1, System.Collections",
    "System.Collections.Generic.ICollection`1, System.Runtime",
    "System.Collections.Generic.IDictionary`2, System.Runtime",
    "System.Collections.Generic.IEnumerable`1, System.Runtime",
    "System.Collections.Generic.IEnumerator`1, System.Runtime",
    "System.Collections.Generic.IList`1, System.Runtime",
    "System.Collections.Generic.IReadOnlyCollection`1, System.Runtime",
    "System.Collections.Generic.IReadOnlyDictionary`2, System.Runtime",
    "System.Collections.Generic.ISet`1, System.Runtime",
    "System.Collections.Generic.KeyValuePair`2, System.Runtime",
    "System.Collections.Generic.List`1, System.Collections",
    "System.Collections.Generic.Queue`1, System.Collections",
    "System.Collections.Generic.Stack`1, System.Collections",
    "System.Convert, System.Private.CoreLib",
    "System.DateTime, System.Private.CoreLib",
    "System.DateTimeKind, System.Private.CoreLib",
    "System.DateTimeOffset, System.Private.CoreLib",
    "System.Decimal, System.Private.CoreLib",
    "System.Double, System.Private.CoreLib",
    "System.Enum, System.Private.CoreLib",
    "System.Exception, System.Private.CoreLib",
    "System.Guid, System.Private.CoreLib",
    "System.Int16, System.Private.CoreLib",
    "System.Int32, System.Private.CoreLib",
    "System.Int64, System.Private.CoreLib",
    "System.IO.StringReader, System.Private.CoreLib",
    "System.IO.StringWriter, System.Private.CoreLib",
    "System.Linq.Enumerable, System.Linq",
    "System.Math, System.Private.CoreLib",
    "System.MidpointRounding, System.Private.CoreLib",
    "System.Net.IPAddress, System.Net.Primitives",
    "System.Net.WebUtility, System.Runtime",
    "System.Nullable, System.Private.CoreLib",
    "System.Random, System.Private.CoreLib",
    "System.SByte, System.Private.CoreLib",
    "System.Security.Cryptography.AsymmetricAlgorithm, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.CipherMode, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.HashAlgorithm, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.HashAlgorithmName, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.HMAC, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.HMACMD5, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.HMACSHA1, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.HMACSHA256, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.HMACSHA384, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.HMACSHA512, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.KeyedHashAlgorithm, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.MD5, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.Oid, System.Security.Cryptography.Encoding",
    "System.Security.Cryptography.PaddingMode, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.RNGCryptoServiceProvider, System.Security.Cryptography.Csp",
    "System.Security.Cryptography.RSA, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.RSAEncryptionPadding, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.RSASignaturePadding, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA1, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA1Managed, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA256, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA256Managed, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA384, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA384Managed, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA512, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SHA512Managed, System.Security.Cryptography.Algorithms",
    "System.Security.Cryptography.SymmetricAlgorithm, System.Security.Cryptography.Primitives",
    "System.Security.Cryptography.X509Certificates.PublicKey, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.RSACertificateExtensions, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.X500DistinguishedName, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.X509Certificate, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.X509Certificate2, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.X509ContentType, System.Security.Cryptography.X509Certificates",
    "System.Security.Cryptography.X509Certificates.X509NameType, System.Security.Cryptography.X509Certificates",
    "System.Single, System.Private.CoreLib",
    "System.String, System.Private.CoreLib",
    "System.StringComparer, System.Private.CoreLib",
    "System.StringComparison, System.Private.CoreLib",
    "System.StringSplitOptions, System.Private.CoreLib",
    "System.Text.Encoding, System.Private.CoreLib",
    "System.Text.RegularExpressions.Capture, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.CaptureCollection, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.Group, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.GroupCollection, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.Match, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.Regex, System.Text.RegularExpressions",
    "System.Text.RegularExpressions.RegexOptions, System.Text.RegularExpressions",
    "System.Text.StringBuilder, System.Private.CoreLib",
    "System.TimeSpan, System.Private.CoreLib",
    "System.TimeZone, System.Private.CoreLib",
    // docs say System.Runtime - but won't load
    //"System.TimeZoneInfo.AdjustmentRule, System.Runtime",
    //"System.TimeZoneInfo.TransitionTime, System.Runtime",
    "System.TimeZoneInfo, System.Private.CoreLib",
    "System.Tuple, System.Private.CoreLib",
    "System.UInt16, System.Private.CoreLib",
    "System.UInt32, System.Private.CoreLib",
    "System.UInt64, System.Private.CoreLib",
    "System.Uri, System.Private.Uri",
    "System.UriPartial, System.Private.Uri",
    "System.Xml.Linq.Extensions, System.Xml.XDocument",
    "System.Xml.Linq.XAttribute, System.Xml.XDocument",
    "System.Xml.Linq.XCData, System.Xml.XDocument",
    "System.Xml.Linq.XComment, System.Xml.XDocument",
    "System.Xml.Linq.XContainer, System.Xml.XDocument",
    "System.Xml.Linq.XDeclaration, System.Xml.XDocument",
    "System.Xml.Linq.XDocument, System.Xml.XDocument",
    "System.Xml.Linq.XDocumentType, System.Xml.XDocument",
    "System.Xml.Linq.XElement, System.Xml.XDocument",
    "System.Xml.Linq.XName, System.Xml.XDocument",
    "System.Xml.Linq.XNamespace, System.Xml.XDocument",
    "System.Xml.Linq.XNode, System.Xml.XDocument",
    "System.Xml.Linq.XNodeDocumentOrderComparer, System.Xml.XDocument",
    "System.Xml.Linq.XNodeEqualityComparer, System.Xml.XDocument",
    "System.Xml.Linq.XObject, System.Xml.XDocument",
    "System.Xml.Linq.XProcessingInstruction, System.Xml.XDocument",
    "System.Xml.Linq.XText, System.Xml.XDocument",
    "System.Xml.XmlNodeType, System.Xml.ReaderWriter"
};

    public static List<MetadataReference> GetMetadataReferences()
    {
        var assemblies = new HashSet<Assembly>([Assembly.GetExecutingAssembly()]);
        foreach (var typeName in SupportedTypes)
        {
            var type = Type.GetType(typeName, throwOnError: false);
            Debug.WriteLineIf(type is null, $"Type name '{typeName}' could not be loaded.");

            if (type is not null && !assemblies.Contains(type.Assembly))
                assemblies.Add(type.Assembly);
        }

        // Add System.Runtime.dll explicitly for Roslyn
        var runtimePath = Path.Combine(
            System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
            "System.Runtime.dll"
        );
        if (File.Exists(runtimePath))
        {
            assemblies.Add(Assembly.LoadFrom(runtimePath));
        }

        return assemblies
            .Distinct()
            .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
            .ToList();
    }

    public static string GetUsings()
    {
        var namespaces = new HashSet<string>();
        foreach (var typeName in SupportedTypes.Select(SplitFullName))
        {
            var lastDot = typeName.LastIndexOf('.');
            if (lastDot > 0)
            {
                var ns = typeName.Substring(0, lastDot);
                namespaces.Add(ns);
            }
        }
        return String.Join(Environment.NewLine, namespaces.Distinct().Select(ns => $"using {ns};"));
    }

    private static string SplitFullName(string fullName)
    {
        var idx = fullName.IndexOf(',');
        return idx >= 0 ? fullName.Substring(0, idx).Trim() : fullName.Trim();
    }
}