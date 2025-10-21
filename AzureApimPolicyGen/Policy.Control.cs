namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#policy-control-and-flow

public interface IControl
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IPolicyDocument Choose(Action<IChooseActions> choose);
}

public interface IChooseActions
{
    IChooseActions When(PolicyExpression condition, Action<IPolicyDocument> whenActions);
    IChooseActions Otherwise(Action<IPolicyDocument> otherwiseActions);
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

}