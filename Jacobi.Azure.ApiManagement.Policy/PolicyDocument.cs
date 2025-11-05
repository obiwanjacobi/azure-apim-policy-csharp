using System.Runtime.CompilerServices;

namespace Jacobi.Azure.ApiManagement.Policy;

internal interface IPolicyDocument : IAuthentication, ICaching, IControl, ICrossDomain, IGraphQL,
    IIngress, IIntegration, ILlm, ILogging, IRouting, ITransformation, IValidation
{ }

public abstract partial class PolicyDocumentBase
{
    private PolicyXmlWriter? _writer;
    internal IDisposable CreateWriter(Stream stream)
    {
        _writer = new PolicyXmlWriter(stream, GetType()?.FullName ?? "<unknown>");
        return _writer;
    }
    internal PolicyXmlWriter Writer => _writer
        ?? throw new InvalidOperationException("PolicyXmlWriter was not initialized.");
}

public abstract partial class PolicyDocument : PolicyDocumentBase,
    IInbound, IBackend, IOutbound, IOnError
{
    private PolicySection _section = PolicySection.None;
    private PolicyScopes _scopes;

    protected PolicyDocument(PolicyScopes policyScopes = PolicyScopes.All)
        => _scopes = policyScopes;

    protected virtual void Inbound(IInbound inbound)
    {
        Writer.Base();
    }

    protected virtual void Backend(IBackend backend)
    {
        Writer.Base();
    }

    protected virtual void Outbound(IOutbound outbound)
    {
        Writer.Base();
    }

    protected virtual void OnError(IOnError onError)
    {
        Writer.Base();
    }

    protected void Comment(string comment)
    {
        Writer.Comment($" {comment} ");
    }

    // ------------------------------------------------------------------------

    IInbound IInbound.Base()
    {
        Writer.Base();
        return this;
    }

    IBackend IBackend.Base()
    {
        Writer.Base();
        return this;
    }

    IOutbound IOutbound.Base()
    {
        Writer.Base();
        return this;
    }

    IOnError IOnError.Base()
    {
        Writer.Base();
        return this;
    }

    // ------------------------------------------------------------------------

    internal void WriteTo(Stream stream)
    {
        using var scope = CreateWriter(stream);

        _section = PolicySection.Inbound;
        Writer.Inbound(() => Inbound(this));

        _section = PolicySection.Backend;
        Writer.Backend(() => Backend(this));

        _section = PolicySection.Outbound;
        Writer.Outbound(() => Outbound(this));

        _section = PolicySection.OnError;
        Writer.OnError(() => OnError(this));

        _section = PolicySection.None;
    }

    // ------------------------------------------------------------------------

    private void AssertSection(PolicySection expected, [CallerMemberName] string callerName = "")
    {
        if (_section != expected)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in section {_section.ToString()}.");
    }
    private void AssertScopes(PolicyScopes policyScopes, [CallerMemberName] string callerName = "")
    {
        if ((_scopes & policyScopes) == 0)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in scope(s) {_scopes.ToString()}.");
    }
}

internal enum PolicySection
{
    None,
    Inbound,
    Backend,
    Outbound,
    OnError
}

[Flags]
public enum PolicyScopes
{
    /// <summary>Not set.</summary>
    None = 0x00,
    /// <summary>Policy at global level.</summary>
    Global = 0x01,
    /// <summary>Policy at workspace level.</summary>
    Workspace = 0x02,
    /// <summary>Policy at product level.</summary>
    Product = 0x04,
    /// <summary>Policy at api level.</summary>
    Api = 0x08,
    /// <summary>Policy at api-operation level.</summary>
    Operation = 0x10,
    /// <summary>Policy at any level (except fragments).</summary>
    All = 0x1F,

    /// <summary>Policy fragment used in other policies.</summary>
    Fragment = 0x80,
}
