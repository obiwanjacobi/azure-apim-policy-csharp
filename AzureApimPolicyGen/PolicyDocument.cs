using System.Runtime.CompilerServices;

namespace AzureApimPolicyGen;

public interface IPolicyDocument
{
    IAuthentication Authentication { get; }
    ICache Cache { get; }

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/check-header-policy</summary>
    IPolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode, PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyDocument Choose(Action<IChooseActions> choose);

    IPolicyDocument Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IPolicyDocument SetBody(PolicyExpression body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IPolicyDocument SetBody(LiquidTemplate body);
}

public abstract partial class PolicyDocument : IPolicyDocument
{
    private PolicySection _section = PolicySection.None;
    private PolicyScopes _scopes = PolicyScopes.None;
    private PolicyXmlWriter? _writer;
    private PolicyXmlWriter Writer => _writer
        ?? throw new InvalidOperationException("PolicyXmlWriter was not initialized.");

    protected PolicyDocument(PolicyScopes policyScopes = PolicyScopes.All)
        => _scopes = policyScopes;

    protected virtual void Inbound()
    {
        Writer.Base();
    }

    protected virtual void Backend()
    {
        Writer.Base();
    }

    protected virtual void Outbound()
    {
        Writer.Base();
    }

    protected virtual void OnError()
    {
        Writer.Base();
    }

    // ------------------------------------------------------------------------

    protected IPolicyDocument Base()
    {
        Writer.Base();
        return this;
    }

    // ------------------------------------------------------------------------

    internal void WriteTo(Stream stream)
    {
        _writer = new PolicyXmlWriter(stream);

        _section = PolicySection.Inbound;
        Writer.Inbound();
        Inbound();
        Writer.EndElement();

        _section = PolicySection.Backend;
        Writer.Backend();
        Backend();
        Writer.EndElement();

        _section = PolicySection.Outbound;
        Writer.Outbound();
        Outbound();
        Writer.EndElement();

        _section = PolicySection.OnError;
        Writer.OnError();
        OnError();
        Writer.EndElement();

        Writer.Close();
    }

    // ------------------------------------------------------------------------

    public IPolicyDocument CheckHeader(PolicyExpression name, PolicyExpression failedCheckHttpCode,
        PolicyExpression failedCheckErrorMessage, PolicyExpression ignoreCase, Action<ICheckHeaderValues>? values = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Action? writeValues = values is null ? null : () => values(new CheckHeaderValues(Writer));
        Writer.CheckHeader(name, failedCheckHttpCode, failedCheckErrorMessage, ignoreCase, writeValues);
        return this;
    }

    public IPolicyDocument Choose(Action<IChooseActions> choose)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions(this)));
        return this;
    }

    public IPolicyDocument EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.EmitMetric(name, @namespace, value, () => dimensions(new EmitMetricDimensions(Writer)));
        return this;
    }

    public IPolicyDocument SetBody(PolicyExpression body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body);
        return this;
    }
    public IPolicyDocument SetBody(LiquidTemplate body)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.SetBody(body, liquidTemplate: true);
        return this;
    }

    // ------------------------------------------------------------------------



    private sealed class CheckHeaderValues : ICheckHeaderValues
    {
        private readonly PolicyXmlWriter _writer;
        internal CheckHeaderValues(PolicyXmlWriter writer) => _writer = writer;

        public ICheckHeaderValues Add(string value)
        {
            _writer.CheckHeaderValue(value);
            return this;
        }
    }

    private sealed class ChooseActions : IChooseActions
    {
        private bool _otherwiseCalled = false;
        private readonly PolicyDocument _document;
        internal ChooseActions(PolicyDocument document) => _document = document;

        public IChooseActions When(PolicyExpression condition, Action<IPolicyDocument> whenActions)
        {
            _document.Writer.ChooseWhen(condition, () => whenActions(_document));
            return this;
        }

        public IChooseActions Otherwise(Action<IPolicyDocument> otherwiseActions)
        {
            if (_otherwiseCalled)
                throw new InvalidOperationException("Otherwise can only be called once.");

            _document.Writer.ChooseOtherwise(() => otherwiseActions(_document));
            _otherwiseCalled = true;
            return this;
        }
    }



    private sealed class EmitMetricDimensions : IEmitMetricDimensions
    {
        private readonly PolicyXmlWriter _writer;
        internal EmitMetricDimensions(PolicyXmlWriter writer) => _writer = writer;

        private int _count;

        public IEmitMetricDimensions Add(string name, string? value)
        {
            if (_count > 5)
                throw new ArgumentOutOfRangeException("<dimension>", "A maximum of 5 dimensions can be specified.");

            _count++;
            _writer.EmitMetricDimension(name, value);
            return this;
        }
    }

    private void AssertSection(PolicySection expected, [CallerMemberName] string callerName = "")
    {
        if (_section != expected)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in section {_section.ToString()}.");
    }
    private void AssertSection(PolicySection[] expected, [CallerMemberName] string callerName = "")
    {
        if (!expected.Contains(_section))
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in section {_section.ToString()}.");
    }
    private void AssertScopes(PolicyScopes policyScopes, [CallerMemberName] string callerName = "")
    {
        if ((_scopes & policyScopes) == 0)
            throw new InvalidOperationException($"Function '{callerName}' cannot be called in scope(s) {_scopes.ToString()}.");
    }
    private static string? CacheTypeToString(CacheType? cacheType)
        => cacheType switch
        {
            CacheType.Internal => "internal",
            CacheType.External => "external",
            CacheType.PreferExternal => "prefer-external",
            _ => null
        };
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
