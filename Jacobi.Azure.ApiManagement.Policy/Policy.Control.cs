namespace Jacobi.Azure.ApiManagement.Policy;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#policy-control-and-flow

internal interface IControl
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyFragment Choose(Action<IChooseActions<IPolicyFragment>> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IPolicyFragment IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IPolicyFragment Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/wait-policy</summary>
    IPolicyFragment Wait(Action<IWaitActions<IPolicyFragment>> actions, PolicyExpression<string>? waitFor = null);
}

public interface IChooseActions<DocumentT>
{
    IChooseActions<DocumentT> When(PolicyExpression<string> condition, Action<DocumentT> whenActions);
    IChooseActions<DocumentT> Otherwise(Action<DocumentT> otherwiseActions);
}

public interface IWaitActions<DocumentT>
{
    IWaitActions<DocumentT> Choose(Action<IChooseActions<DocumentT>> choose);
    IWaitActions<DocumentT> CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);
    IWaitActions<DocumentT> SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);
}

partial class PolicyDocumentBase
{
    internal PolicyDocumentBase Choose(IPolicyFragment document, Action<IChooseActions<IPolicyFragment>> choose)
    {
        Writer.Choose(() => choose(new ChooseActions<IPolicyFragment>(document, Writer)));
        return this;
    }

    internal sealed class ChooseActions<DocumentT> : IChooseActions<DocumentT>
    {
        private bool _otherwiseCalled = false;
        private readonly DocumentT _document;
        private readonly PolicyXmlWriter _writer;

        internal ChooseActions(DocumentT document, PolicyXmlWriter writer)
        {
            _document = document;
            _writer = writer;
        }
        public IChooseActions<DocumentT> When(PolicyExpression<string> condition, Action<DocumentT> whenActions)
        {
            _writer.ChooseWhen(condition, () => whenActions(_document));
            return this;
        }

        public IChooseActions<DocumentT> Otherwise(Action<DocumentT> otherwiseActions)
        {
            if (_otherwiseCalled)
                throw new InvalidOperationException("Otherwise can only be called once.");

            _writer.ChooseOtherwise(() => otherwiseActions(_document));
            _otherwiseCalled = true;
            return this;
        }
    }

    internal PolicyDocumentBase IncludeFragment(string fragmentId)
    {
        Writer.IncludeFragment(fragmentId);
        return this;
    }

    internal PolicyDocumentBase Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null)
    {
        Writer.Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds.ToString(), firstFastRetry);
        return this;
    }

    internal PolicyDocumentBase Wait(IPolicyFragment document, Action<IWaitActions<IPolicyFragment>> actions, PolicyExpression<string>? waitFor)
    {
        Writer.Wait(waitFor, () => actions(new WaitActions(document)));
        return this;
    }

    private sealed class WaitActions : IWaitActions<IPolicyFragment>
    {
        private readonly IPolicyFragment _document;
        public WaitActions(IPolicyFragment document) { _document = document; }

        IWaitActions<IPolicyFragment> IWaitActions<IPolicyFragment>.Choose(Action<IChooseActions<IPolicyFragment>> choose)
        {
            _document.Choose(choose);
            return this;
        }
        IWaitActions<IPolicyFragment> IWaitActions<IPolicyFragment>.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
        {
            _document.CacheLookupValue(variableName, key, defaultValue, cacheType);
            return this;
        }
        IWaitActions<IPolicyFragment> IWaitActions<IPolicyFragment>.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
        {
            _document.SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
            return this;
        }
    }
}

partial class PolicyDocument
{
    IInbound IInbound.Choose(Action<IChooseActions<IInbound>> choose)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions<IInbound>(this, Writer)));
        return this;
    }
    IBackend IBackend.Choose(Action<IChooseActions<IBackend>> choose)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions<IBackend>(this, Writer)));
        return this;
    }
    IOutbound IOutbound.Choose(Action<IChooseActions<IOutbound>> choose)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions<IOutbound>(this, Writer)));
        return this;
    }
    IOnError IOnError.Choose(Action<IChooseActions<IOnError>> choose)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        Writer.Choose(() => choose(new ChooseActions<IOnError>(this, Writer)));
        return this;
    }

    IInbound IInbound.IncludeFragment(string fragmentId)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        IncludeFragment(fragmentId);
        return this;
    }
    IBackend IBackend.IncludeFragment(string fragmentId)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        IncludeFragment(fragmentId);
        return this;
    }
    IOutbound IOutbound.IncludeFragment(string fragmentId)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        IncludeFragment(fragmentId);
        return this;
    }
    IOnError IOnError.IncludeFragment(string fragmentId)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        IncludeFragment(fragmentId);
        return this;
    }

    IInbound IInbound.Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds, int? deltaSeconds, PolicyExpression<bool>? firstFastRetry)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds, firstFastRetry);
        return this;
    }
    IBackend IBackend.Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds, int? deltaSeconds, PolicyExpression<bool>? firstFastRetry)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds, firstFastRetry);
        return this;
    }
    IOutbound IOutbound.Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds, int? deltaSeconds, PolicyExpression<bool>? firstFastRetry)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds, firstFastRetry);
        return this;
    }
    IOnError IOnError.Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds, int? deltaSeconds, PolicyExpression<bool>? firstFastRetry)
    {
        AssertSection(PolicySection.OnError);
        AssertScopes(PolicyScopes.All);
        Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds, firstFastRetry);
        return this;
    }

    IInbound IInbound.Wait(Action<IWaitActions<IInbound>> actions, PolicyExpression<string>? waitFor)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.All);
        Writer.Wait(waitFor, () => actions(new WaitActions(this)));
        return this;
    }
    IBackend IBackend.Wait(Action<IWaitActions<IBackend>> actions, PolicyExpression<string>? waitFor)
    {
        AssertSection(PolicySection.Backend);
        AssertScopes(PolicyScopes.All);
        Writer.Wait(waitFor, () => actions(new WaitActions(this)));
        return this;
    }
    IOutbound IOutbound.Wait(Action<IWaitActions<IOutbound>> actions, PolicyExpression<string>? waitFor)
    {
        AssertSection(PolicySection.Outbound);
        AssertScopes(PolicyScopes.All);
        Writer.Wait(waitFor, () => actions(new WaitActions(this)));
        return this;
    }

    private sealed class WaitActions : IWaitActions<IInbound>, IWaitActions<IBackend>, IWaitActions<IOutbound>
    {
        private readonly PolicyDocument _document;
        public WaitActions(PolicyDocument document) { _document = document; }

        IWaitActions<IInbound> IWaitActions<IInbound>.Choose(Action<IChooseActions<IInbound>> choose)
        {
            ((IInbound)_document).Choose(choose);
            return this;
        }
        IWaitActions<IInbound> IWaitActions<IInbound>.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
        {
            _document.CacheLookupValue(variableName, key, defaultValue, cacheType);
            return this;
        }
        IWaitActions<IInbound> IWaitActions<IInbound>.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
        {
            _document.SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
            return this;
        }

        IWaitActions<IBackend> IWaitActions<IBackend>.Choose(Action<IChooseActions<IBackend>> choose)
        {
            ((IBackend)_document).Choose(choose);
            return this;
        }
        IWaitActions<IBackend> IWaitActions<IBackend>.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
        {
            _document.CacheLookupValue(variableName, key, defaultValue, cacheType);
            return this;
        }
        IWaitActions<IBackend> IWaitActions<IBackend>.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
        {
            _document.SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
            return this;
        }

        IWaitActions<IOutbound> IWaitActions<IOutbound>.Choose(Action<IChooseActions<IOutbound>> choose)
        {
            ((IOutbound)_document).Choose(choose);
            return this;
        }
        IWaitActions<IOutbound> IWaitActions<IOutbound>.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
        {
            _document.CacheLookupValue(variableName, key, defaultValue, cacheType);
            return this;
        }

        IWaitActions<IOutbound> IWaitActions<IOutbound>.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
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