using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace AzureApimPolicyGen;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

public static class Context
{
    public static TimeSpan Elapsed { get; }
    public static DateTime Timestamp { get; }

    public static IContextDeployment Deployment { get; }

    public static IContextProduct Product { get; }
    public static IContextApiVersion Api { get; }
    public static IContextOperation Operation { get; }
    public static IContextSubscription Subscription { get; }

    public static Guid RequestId { get; }
    public static IContextRequest Request { get; }
    public static IContextResponse Response { get; }

    public static IContextUser User { get; }

    public static IReadOnlyDictionary<string, object> Variables { get; }

    public static IContextLastError LastError { get; }

    public static bool Tracing { get; }
    public static Action<string> Trace { get; }
}

public interface IContextApi
{
    string Id { get; }
    string Name { get; }
    string Path { get; }
    IEnumerable<string> Protocols { get; }
    IContextUrl ServiceUrl { get; }
    IContextSubscriptionKeyParameterNames SubscriptionKeyParameterNames { get; }
}
public interface IContextApiVersion
{
    bool IsCurrentRevision { get; }
    string Revision { get; }
    string Version { get; }
}

public interface IContextProduct
{
    string Id { get; }
    string Name { get; }
    ProductState State { get; }
    int? SubscriptionLimit { get; }
    bool SubscriptionRequired { get; }
    bool ApprovalRequired { get; }

    IEnumerable<IContextApi> Apis { get; }
    IEnumerable<IContextGroup> Groups { get; }
}

public enum ProductState { NotPublished, Published }

public interface IContextOperation
{
    string Id { get; }
    string Name { get; }
    string Method { get; }
    string UrlTemplate { get; }
}

public interface IContextSubscription
{
    string Id { get; }
    string Key { get; }
    string PrimaryKey { get; }
    string SecondaryKey { get; }
    string Name { get; }

    DateTime CreatedDate { get; }
    DateTime? StartDate { get; }
    DateTime? EndDate { get; }
}

public interface IContextSubscriptionKeyParameterNames
{
    string Query { get; }
    string Header { get; }
}

public interface IContextDeployment
{
    string ServiceId { get; }
    string ServiceName { get; }
    string Region { get; }
    string GatewayId { get; }
    IReadOnlyDictionary<string, X509Certificate2> Certificates { get; }
}

public interface IContextGroup
{
    string Id { get; }
    string Name { get; }
}

public interface IContextRequest
{
    string Method { get; }
    string IpAddress { get; }
    IContextUrl OriginalUrl { get; }
    IContextUrl Url { get; }

    IReadOnlyDictionary<string, string[]> Headers { get; }
    IContextMessageBody? Body { get; }

    IReadOnlyDictionary<string, string> MatchedParameters { get; }
    IContextPrivateEndpointConnection? PrivateEndpointConnection { get; }
    X509Certificate2? Certificate { get; }
}

public interface IContextResponse
{
    int StatusCode { get; }
    string StatusReason { get; }
    IReadOnlyDictionary<string, string[]> Headers { get; }
    IContextMessageBody Body { get; }
}

public interface IContextMessageBody
{
    T As<T>(bool preserveContent = false);
    IDictionary<string, IList<string>> AsFormUrlEncodedContent(bool preserveContent = false);
}

public interface IContextPrivateEndpointConnection
{
    string Name { get; }
    string MemberName { get; }
    string GroupId { get; }
}

public interface IContextUser
{
    string Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string Note { get; }
    DateTime RegistrationDate { get; }

    IEnumerable<IContextGroup> Groups { get; }
    IEnumerable<IContextUserIdentity> Identities { get; }
}

public interface IContextUserIdentity
{
    string Id { get; }
    string Provider { get; }
}

public interface IContextLastError
{
    string PolicyId { get; }
    string Source { get; }
    string Reason { get; }
    string Message { get; }
    string Scope { get; }
    string Section { get; }
    string Path { get; }
}

public interface IContextUrl
{
    string Scheme { get; }
    string Host { get; }
    string Port { get; }
    string Path { get; }
    string QueryString { get; }
    IReadOnlyDictionary<string, string[]> Query { get; }
}

public interface IContextJwt
{
    string Id { get; }
    public string Subject { get; }
    public string Type { get; }
    public string Issuer { get; }
    public DateTime? ExpirationTime { get; }
    public DateTime? NotBefore { get; }
    public DateTime? IssuedAt { get; }
    public string Algorithm { get; }
    public IEnumerable<string> Audiences { get; }
    public IReadOnlyDictionary<string, string[]> Claims { get; }
}

public static class ContextExtensions
{
    public static string? GetValueOrDefault(this IReadOnlyDictionary<string, string[]> dictionary, string headerName) => null;
    public static string GetValueOrDefault(this IReadOnlyDictionary<string, string[]> dictionary, string headerName, string defaultValue) => defaultValue;
    public static T? GetValueOrDefault<T>(this IReadOnlyDictionary<string, object> dictionary, string variableName) => default;
    public static T GetValueOrDefault<T>(this IReadOnlyDictionary<string, object> dictionary, string variableName, T defaultValue) => defaultValue;

    public static string? ToQueryString(this IDictionary<string, IList<string>>? dictionary) => null;
    public static string? ToFormUrlEncodedContent(this IDictionary<string, IList<string>>? dictionary) => null;

    public static IContextBasicCredentials? AsBasic(this string? value) => null;
    public static bool TryParseBasic(this string? value, [MaybeNullWhen(false)] out IContextBasicCredentials credentials)
    {
        credentials = null;
        return false;
    }

    public static IContextJwt? AsJwt(this string? value) => null;
    public static bool TryParseJwt(this string? value, [MaybeNullWhen(false)] out IContextJwt token)
    {
        token = null;
        return false;
    }
}

public interface IContextBasicCredentials
{
    public string Username { get; }
    public string Password { get; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.