using System.Xml.Linq;
using AzureApimPolicyGen;

namespace AzureApimPolicy.Tests;

internal static class PolicyXml
{
    //public static string ToString<T>()
    //    where T : PolicyDocument
    //{
    //    var doc = Activator.CreateInstance<T>();

    //    var stream = new MemoryStream();
    //    doc.WriteTo(stream);

    //    stream.Position = 0;

    //    var reader = new StreamReader(stream);
    //    return reader.ReadToEnd();
    //}

    public static XDocument ToXDocument<T>()
        where T : PolicyDocument
    {
        var doc = Activator.CreateInstance<T>();
        var stream = new MemoryStream();
        doc.WriteTo(stream);
        stream.Position = 0;
        return XDocument.Load(stream);
    }
}
