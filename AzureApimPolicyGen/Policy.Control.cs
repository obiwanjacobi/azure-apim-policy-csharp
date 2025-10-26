namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#policy-control-and-flow

public interface IControl
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyDocument Choose(Action<IChooseActions> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IPolicyDocument IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IPolicyDocument Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/wait-policy</summary>
    IPolicyDocument Wait(Action<IWaitActions> actions, PolicyExpression<string>? waitFor = null);
}

public interface IChooseActions
{
    IChooseActions When(PolicyExpression<string> condition, Action<IPolicyDocument> whenActions);
    IChooseActions Otherwise(Action<IPolicyDocument> otherwiseActions);
}

public interface IWaitActions
{
    IWaitActions Choose(Action<IChooseActions> choose);
    IWaitActions CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);
    IWaitActions SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);
}

partial class PolicyDocument
{
    public IPolicyDocument Choose(Action<IChooseActions> choose)
    {
        // allowed in all sections
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions(this)));
        return this;
    }

    private sealed class ChooseActions : IChooseActions
    {
        private bool _otherwiseCalled = false;
        private readonly PolicyDocument _document;
        internal ChooseActions(PolicyDocument document) => _document = document;

        public IChooseActions When(PolicyExpression<string> condition, Action<IPolicyDocument> whenActions)
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

    public IPolicyDocument IncludeFragment(string fragmentId)
    {
        AssertScopes(PolicyScopes.All);
        Writer.IncludeFragment(fragmentId);
        return this;
    }

    public IPolicyDocument Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null)
    {
        AssertScopes(PolicyScopes.All);
        Writer.Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds.ToString(), firstFastRetry);
        return this;
    }

    public IPolicyDocument Wait(Action<IWaitActions> actions, PolicyExpression<string>? waitFor = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.Backend]);
        AssertScopes(PolicyScopes.All);
        Writer.Wait(waitFor, () => actions(new WaitActions(this)));
        return this;
    }

    private sealed class WaitActions : IWaitActions
    {
        private readonly IPolicyDocument _document;
        public WaitActions(IPolicyDocument document) { _document = document; }

        public IWaitActions Choose(Action<IChooseActions> choose)
        {
            _document.Choose(choose);
            return this;
        }

        public IWaitActions CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null)
        {
            _document.CacheLookupValue(variableName, key, defaultValue, cacheType);
            return this;
        }

        public IWaitActions SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null)
        {
            _document.SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
            return this;
        }
    }
}

partial class PolicyXmlWriter
{
    public void Choose(Action actions)
    {
        _xmlWriter.WriteStartElement("choose");
        actions();
        _xmlWriter.WriteEndElement();
    }

    internal void ChooseWhen(string condition, Action actions)
    {
        _xmlWriter.WriteStartElement("when");
        _xmlWriter.WriteAttributeString("condition", condition);
        actions();
        _xmlWriter.WriteEndElement();
    }

    internal void ChooseOtherwise(Action actions)
    {
        _xmlWriter.WriteStartElement("otherwise");
        actions();
        _xmlWriter.WriteEndElement();
    }

    public void IncludeFragment(string fragmentId)
    {
        _xmlWriter.WriteStartElement("include-fragment");
        _xmlWriter.WriteAttributeString("fragment-id", fragmentId);
        _xmlWriter.WriteEndElement();
    }

    public void Retry(string condition, string count, string interval, string? maxInterval, string? delta, string? firstFastRetry)
    {
        _xmlWriter.WriteStartElement("retry");
        _xmlWriter.WriteAttributeString("condition", condition);
        _xmlWriter.WriteAttributeString("count", count);
        _xmlWriter.WriteAttributeString("interval", interval);
        _xmlWriter.WriteAttributeStringOpt("max-interval", maxInterval);
        _xmlWriter.WriteAttributeStringOpt("delta", delta);
        _xmlWriter.WriteAttributeStringOpt("first-fast-retry", firstFastRetry);
        _xmlWriter.WriteEndElement();
    }

    public void Wait(string? waitFor, Action actions)
    {
        _xmlWriter.WriteStartElement("wait");
        _xmlWriter.WriteAttributeStringOpt("for", waitFor);
        actions();
        _xmlWriter.WriteEndElement();
    }
}