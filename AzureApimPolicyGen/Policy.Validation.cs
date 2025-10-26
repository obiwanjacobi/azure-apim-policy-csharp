namespace AzureApimPolicyGen;

// https://learn.microsoft.com/en-us/azure/api-management/api-management-policies#content-validation

public interface IValidation
{
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-content-policy</summary>
    IPolicyDocument ValidateContent(PolicyExpression unspecifiedContentTypeAction, PolicyExpression maxSizeBytes, PolicyExpression sizeExceedAction, PolicyVariable? errorsVariableName = null, Action<IValidateContentActions>? validateActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-headers-policy</summary>
    IPolicyDocument ValidateHeaders(PolicyExpression specifiedHeaderAction, PolicyExpression unspecifiedHeaderAction, PolicyVariable? errorsVariableName = null, Action<IValidateHeaderActions>? headers = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-odata-request-policy</summary>
    IPolicyDocument ValidateODataRequest(PolicyVariable? errorVariableName = null, string? defaultODataVersion = null, string? minODataVersion = null, string? maxODataVersion = null, int? maxSizeBytes = null);
}

public interface IValidateContentActions
{
    IValidateContentActions ContentTypeMap(string? anyContentTypeValue = null, string? missingContentTypeValue = null, Action<IValidateContentTypeMapType>? types = null);
    IValidateContentActions Content(ValidateContentAs validateAs, string? type = null, string? schemaId = null, string? schemaRef = null, bool? allowAdditionProperties = null, bool? caseInsensitivePropertyNames = null);
}

public interface IValidateContentTypeMapType
{
    IValidateContentTypeMapType Add(string from, string to);
    IValidateContentTypeMapType Add(PolicyExpression when, string to);
}

public enum ValidateContentAs
{
    Xml, Soap, Json
}

public interface IValidateHeaderActions
{
    IValidateHeaderActions Add(string name, PolicyExpression action);
}

partial class PolicyDocument
{
    public IPolicyDocument ValidateContent(PolicyExpression unspecifiedContentTypeAction, PolicyExpression maxSizeBytes, PolicyExpression sizeExceedAction, PolicyVariable? errorsVariableName = null, Action<IValidateContentActions>? validateActions = null)
    {
        AssertSection([PolicySection.Inbound, PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Action? writeActions = validateActions is null ? null : () => validateActions(new ValidateContentActions(Writer));
        Writer.ValidateContent(unspecifiedContentTypeAction, maxSizeBytes, sizeExceedAction, errorsVariableName, writeActions);
        return this;
    }

    private sealed class ValidateContentActions : IValidateContentActions, IValidateContentTypeMapType
    {
        private readonly PolicyXmlWriter _writer;
        public ValidateContentActions(PolicyXmlWriter writer) { _writer = writer; }

        public IValidateContentActions ContentTypeMap(string? anyContentTypeValue = null, string? missingContentTypeValue = null, Action<IValidateContentTypeMapType>? types = null)
        {
            Action? writeTypes = types is null ? null : () => types(this);
            _writer.ValidateContentTypeMap(anyContentTypeValue, missingContentTypeValue, writeTypes);
            return this;
        }

        public IValidateContentActions Content(ValidateContentAs validateAs, string? type = null, string? schemaId = null, string? schemaRef = null, bool? allowAdditionProperties = null, bool? caseInsensitivePropertyNames = null)
        {
            _writer.ValidateContentContent(ValidateAsToString(validateAs), type, schemaId, schemaRef, allowAdditionProperties, caseInsensitivePropertyNames);
            return this;

            static string ValidateAsToString(ValidateContentAs validateAs)
                => validateAs switch
                {
                    ValidateContentAs.Json => "json",
                    ValidateContentAs.Soap => "soap",
                    ValidateContentAs.Xml => "xml",
                    _ => "json"
                };
        }

        public IValidateContentTypeMapType Add(string from, string to)
        {
            _writer.ValidateContentTypeMapType(null, from, to);
            return this;
        }

        public IValidateContentTypeMapType Add(PolicyExpression when, string to)
        {
            _writer.ValidateContentTypeMapType(when, null, to);
            return this;
        }
    }

    public IPolicyDocument ValidateHeaders(PolicyExpression specifiedHeaderAction, PolicyExpression unspecifiedHeaderAction, PolicyVariable? errorsVariableName = null, Action<IValidateHeaderActions>? headers = null)
    {
        AssertSection([PolicySection.Outbound, PolicySection.OnError]);
        AssertScopes(PolicyScopes.All);
        Action? writeHeaders = headers is null ? null : () => headers(new ValidateHeaderActions(Writer));
        Writer.ValidateHeaders(specifiedHeaderAction, unspecifiedHeaderAction, errorsVariableName, writeHeaders);
        return this;
    }

    private sealed class ValidateHeaderActions : IValidateHeaderActions
    {
        private readonly PolicyXmlWriter _writer;
        public ValidateHeaderActions(PolicyXmlWriter writer) { _writer = writer; }

        public IValidateHeaderActions Add(string name, PolicyExpression action)
        {
            _writer.ValidateHeaderName(name, action);
            return this;
        }
    }

    public IPolicyDocument ValidateODataRequest(PolicyVariable? errorVariableName = null, string? defaultODataVersion = null, string? minODataVersion = null, string? maxODataVersion = null, int? maxSizeBytes = null)
    {
        AssertSection(PolicySection.Inbound);
        AssertScopes(PolicyScopes.Global | PolicyScopes.Workspace | PolicyScopes.Product | PolicyScopes.Api);
        Writer.ValidateODataRequest(errorVariableName, defaultODataVersion, minODataVersion, maxODataVersion, maxSizeBytes.ToString());
        return this;
    }
}

partial class PolicyXmlWriter
{
    public void ValidateContent(string unspecifiedContentTypeAction, string maxSizeBytes, string sizeExceedAction, string? errorsVariableName, Action? writeActions)
    {
        _xmlWriter.WriteStartElement("validate-content");
        _xmlWriter.WriteAttributeString("unspecified-content-type-action", unspecifiedContentTypeAction);
        _xmlWriter.WriteAttributeString("max-size", maxSizeBytes);
        _xmlWriter.WriteAttributeString("size-exceed-action", sizeExceedAction);
        _xmlWriter.WriteAttributeStringOpt("errors-variable-name", errorsVariableName);
        if (writeActions is not null) writeActions();
        _xmlWriter.WriteEndElement();
    }

    internal void ValidateContentContent(string validateAs, string? type, string? schemaId, string? schemaRef, bool? allowAdditionProperties, bool? caseInsensitivePropertyNames)
    {
        _xmlWriter.WriteStartElement("content");
        _xmlWriter.WriteAttributeString("validate-as", validateAs);
        _xmlWriter.WriteAttributeStringOpt("type", type);
        _xmlWriter.WriteAttributeStringOpt("schema-id", schemaId);
        _xmlWriter.WriteAttributeStringOpt("schema-ref", schemaRef);
        _xmlWriter.WriteAttributeStringOpt("allow-additional-properties", BoolValue(allowAdditionProperties));
        _xmlWriter.WriteAttributeStringOpt("case-insensitive-property-names", BoolValue(caseInsensitivePropertyNames));
        _xmlWriter.WriteEndElement();
    }

    internal void ValidateContentTypeMap(string? anyContentTypeValue, string? missingContentTypeValue, Action? writeTypes)
    {
        _xmlWriter.WriteStartElement("content-type-map");
        _xmlWriter.WriteAttributeStringOpt("any-content-type-value", anyContentTypeValue);
        _xmlWriter.WriteAttributeStringOpt("missing-content-type-value", missingContentTypeValue);
        if (writeTypes is not null) writeTypes();
        _xmlWriter.WriteEndElement();
    }

    internal void ValidateContentTypeMapType(string? when, string? from, string to)
    {
        _xmlWriter.WriteStartElement("type");
        _xmlWriter.WriteAttributeStringOpt("when", when);
        _xmlWriter.WriteAttributeStringOpt("from", from);
        _xmlWriter.WriteAttributeString("to", to);
        _xmlWriter.WriteEndElement();
    }

    public void ValidateHeaders(string specifiedHeaderAction, string unspecifiedHeaderAction, string? errorsVariableName, Action? writeHeaders)
    {
        _xmlWriter.WriteStartElement("validate-headers");
        _xmlWriter.WriteAttributeString("specified-header-action", specifiedHeaderAction);
        _xmlWriter.WriteAttributeString("unspecified-header-action", unspecifiedHeaderAction);
        _xmlWriter.WriteAttributeStringOpt("errors-variable-name", errorsVariableName);
        if (writeHeaders is not null) writeHeaders();
        _xmlWriter.WriteEndElement();
    }
    internal void ValidateHeaderName(string name, string action)
    {
        _xmlWriter.WriteStartElement("header");
        _xmlWriter.WriteAttributeString("name", name);
        _xmlWriter.WriteAttributeString("action", action);
        _xmlWriter.WriteEndElement();
    }

    public void ValidateODataRequest(string? errorVariableName, string? defaultODataVersion, string? minODataVersion, string? maxODataVersion, string? maxSize)
    {
        _xmlWriter.WriteStartElement("validate-odata-request");
        _xmlWriter.WriteAttributeStringOpt("error-variable-name", errorVariableName);
        _xmlWriter.WriteAttributeStringOpt("default-odata-version", defaultODataVersion);
        _xmlWriter.WriteAttributeStringOpt("min-odata-version", minODataVersion);
        _xmlWriter.WriteAttributeStringOpt("max-odata-version", maxODataVersion);
        _xmlWriter.WriteAttributeStringOpt("max-size", maxSize);
        _xmlWriter.WriteEndElement();
    }
}
