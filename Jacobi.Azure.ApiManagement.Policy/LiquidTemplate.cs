namespace Jacobi.Azure.ApiManagement.Policy;

/// <summary>
/// body.
/// context.
///     Request.
///         Url
///         Method
///         OriginalMethod
///         OriginalUrl
///         IpAddress
///         MatchedParameters
///         HasBody
///         ClientCertificates
///         Headers
///     Response.
///         StatusCode
///         Method
///         Headers
/// </summary>
// https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body
public struct LiquidTemplate
{
    private string _template;

    public LiquidTemplate(string template) => _template = template;

    // ... to prevent ambiguous refs with PolicyExpression
    //public static implicit operator LiquidTemplate(string template) => new(template);
    public static implicit operator string(LiquidTemplate template) => template._template;

    public static LiquidTemplate From(string template) => new(template);
}
