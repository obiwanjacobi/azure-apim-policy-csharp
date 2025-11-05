namespace Jacobi.Azure.ApiManagement.Policy;

partial class PolicyDocument : IInbound, IBackend, IOutbound, IOnError
{ }

public interface IInbound
{
    IInbound Base();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy</summary>
    IInbound AuthenticationBasic(PolicyExpression<string> username, PolicyExpression<string> password);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy</summary>
    IInbound AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body = null, PolicyExpression<string>? password = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy</summary>
    IInbound AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId = null, PolicyVariable? outputTokenVariableName = null, bool? ignoreError = false);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/check-header-policy</summary>
    IInbound CheckHeader(PolicyExpression<string> name, PolicyExpression<int> failedCheckHttpCode, PolicyExpression<string> failedCheckErrorMessage, PolicyExpression<bool> ignoreCase, Action<ICheckHeaderValues>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/get-authorization-context-policy</summary>
    IInbound GetAuthorizationContext(PolicyExpression<string> providerId, PolicyExpression<string> authorizationId, PolicyVariable contextVariableName, PolicyExpression<string>? identity = null, PolicyExpression<bool>? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/ip-filter-policy</summary>
    IInbound IpFilter(PolicyExpression<string> action, Action<IIpFilterAddress> address);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-azure-ad-token-policy</summary>
    IInbound ValidateAzureAdToken(PolicyExpression<string> tenantIdOrUrl, PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, string? authenticationEndpoint = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateAzureAdTokenActions>? validationActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-client-certificate-policy</summary>
    IInbound ValidateClientCertificate(bool? validateRevocation = null, bool? validateTrust = null, bool? validateNotBefore = null, bool? validateNotAfter = null, bool? ignoreError = null, Action<IValidateClientCertificateIdentities>? identities = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-jwt-policy</summary>
    IInbound ValidateJwt(PolicyExpression<string>? headerName = null, PolicyExpression<string>? queryParameterName = null, PolicyExpression<string>? tokenValue = null, PolicyExpression<int>? failedValidationHttpCode = null, PolicyExpression<string>? failedValidationErrorMessage = null, PolicyExpression<bool>? requireExpirationTime = null, PolicyExpression<string>? requireScheme = null, PolicyExpression<bool>? requireSignedTokens = null, PolicyExpression<int>? clockSkewSeconds = null, PolicyVariable? outputTokenVariableName = null, Action<IValidateJwtActions>? jwtActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy</summary>
    IInbound CacheLookup(PolicyExpression<bool> varyByDeveloper, PolicyExpression<bool> VarByDeveloperGroup, PolicyExpression<bool>? allowPrivateResponseCaching = null, CacheType? cacheType = null, PolicyExpression<string>? downstreamCacheType = null, PolicyExpression<bool>? mustRevalidate = null, Action<ICacheLookupVaryBy>? varyBy = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IInbound CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IInbound CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IInbound CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IInbound Choose(Action<IChooseActions<IInbound>> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IInbound IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IInbound Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/wait-policy</summary>
    IInbound Wait(Action<IWaitActions<IInbound>> actions, PolicyExpression<string>? waitFor = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cors-policy</summary>
    IInbound Cors(Action<ICorsActions> cors, bool? allowCredentials = null, bool? terminateUnmatchedRequests = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cross-domain-policy</summary>
    /// <remarks>Xml Schema: https://www.adobe.com/xml/schemas/PolicyFile.xsd</remarks>
    IInbound CrossDomain(Action<ICrossDomainActions> actions, CrossDomainPolicies? permittedCrossDomainPolicies = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IInbound LimitConcurrency(PolicyExpression<string> key, int maxCount, Action<IInbound> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/quota-policy</summary>
    IInbound Quota(int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApi> apis);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/quota-by-key-policy</summary>
    IInbound QuotaByKey(PolicyExpression<string> counterKey, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, PolicyExpression<int>? incrementCount = null, PolicyExpression<string>? incrementCondition = null, DateTime? firstPeriodStart = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rate-limit-policy</summary>
    IInbound RateLimit(int numberOfCalls, int renewalPeriodSeconds, PolicyVariable? retryAfterVariableName = null, string? retryAfterHeaderName = null, PolicyVariable? remainingCallsVariableName = null, string? remainingCallsHeaderName = null, string? totalCallsHeaderName = null, Action<IRateLimitApi>? apis = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rate-limit-by-key-policy</summary>
    IInbound RateLimitByKey(PolicyExpression<string> counterKey, PolicyExpression<int> numberOfCalls, PolicyExpression<int> renewalPeriodSeconds, PolicyExpression<int>? incrementCount = null, PolicyExpression<string>? incrementCondition = null, PolicyVariable? retryAfterVariableName = null, string? retryAfterHeaderName = null, PolicyVariable? remainingCallsVariableName = null, string? remainingCallsHeaderName = null, string? totalCallsHeaderName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IInbound PublishToDapr(PolicyExpression<string> message, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression<int>? timeoutSeconds = null, string? contentType = null, bool? ignoreError = null);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy</summary>
    IInbound PublishToDapr(LiquidTemplate template, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName = null, PolicyVariable? responseVariableName = null, PolicyExpression<int>? timeoutSeconds = null, string? contentType = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IInbound SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-service-bus-message-policy</summary>
    IInbound SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties = null, PolicyExpression<string>? queueName = null, PolicyExpression<string>? topicName = null, PolicyExpression<string>? clientId = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-request-policy</summary>
    IInbound SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-dapr-policy</summary>
    IInbound SetBackendService(PolicyExpression<string> daprAppId, PolicyExpression<string> daprMethod, PolicyExpression<string>? daprNamespace = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IInbound EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IInbound LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/trace-policy</summary>
    IInbound Trace(string source, PolicyExpression<string> message, TraceSeverity severity = TraceSeverity.Verbose, string? metadataName = null, string? metadataValue = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/proxy-policy</summary>
    IInbound Proxy(PolicyExpression<string> url, PolicyExpression<string>? username = null, PolicyExpression<string>? password = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IInbound SetBackendService(PolicyExpression<string> baseUrl);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IInbound SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition = null, PolicyExpression<string>? sfServiceInstanceName = null, PolicyExpression<string>? sfPartitionKey = null, PolicyExpression<string>? sfListenerName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IInbound FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IInbound JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IInbound MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy</summary>
    IInbound RedirectContentUrls();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IInbound ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/rewrite-uri-policy</summary>
    IInbound RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IInbound SetBody(PolicyExpression<string> body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IInbound SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IInbound SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-method-policy</summary>
    IInbound SetMethod(PolicyExpression<string> method);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IInbound SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy</summary>
    IInbound SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IInbound SetVariable(string name, PolicyExpression<string> value);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy</summary>
    IInbound XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xsl-transform-policy</summary>
    IInbound XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-content-policy</summary>
    IInbound ValidateContent(PolicyExpression<string> unspecifiedContentTypeAction, PolicyExpression<int> maxSizeBytes, PolicyExpression<string> sizeExceedAction, PolicyVariable? errorsVariableName = null, Action<IValidateContentActions>? validateActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-odata-request-policy</summary>
    IInbound ValidateODataRequest(PolicyVariable? errorVariableName = null, string? defaultODataVersion = null, string? minODataVersion = null, string? maxODataVersion = null, int? maxSizeBytes = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-parameters-policy</summary>
    IInbound ValidateParameters(PolicyExpression<string> specifiedParameterAction, PolicyExpression<string> unspecifiedParameterAction, PolicyVariable? errorVariableName = null, Action<IValidateParameterActions>? parameterActions = null);
}

public interface IBackend
{
    IBackend Base();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IBackend CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IBackend CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IBackend CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IBackend Choose(Action<IChooseActions<IBackend>> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IBackend IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IBackend Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/wait-policy</summary>
    IBackend Wait(Action<IWaitActions<IBackend>> actions, PolicyExpression<string>? waitFor = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IBackend LimitConcurrency(PolicyExpression<string> key, int maxCount, Action<IBackend> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IBackend SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-request-policy</summary>
    IBackend SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IBackend EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IBackend LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/trace-policy</summary>
    IBackend Trace(string source, PolicyExpression<string> message, TraceSeverity severity = TraceSeverity.Verbose, string? metadataName = null, string? metadataValue = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/forward-request-policy</summary>
    IBackend ForwardRequest(HttpVersion? httpVersion = null, PolicyExpression<int>? timeoutSeconds = null, PolicyExpression<int>? timeoutMilliseconds = null, PolicyExpression<int>? continueTimeout = null, bool? followRedirects = null, bool? bufferRequestBody = null, bool? bufferResponse = null, bool? failOnErrorStatusCode = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IBackend SetBackendService(PolicyExpression<string> baseUrl);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy</summary>
    IBackend SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition = null, PolicyExpression<string>? sfServiceInstanceName = null, PolicyExpression<string>? sfPartitionKey = null, PolicyExpression<string>? sfListenerName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IBackend FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IBackend ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IBackend SetBody(PolicyExpression<string> body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IBackend SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IBackend SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IBackend SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy</summary>
    IBackend SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IBackend SetVariable(string name, PolicyExpression<string> value);

}

public interface IOutbound
{
    IOutbound Base();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IOutbound CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy</summary>
    IOutbound CacheStore(PolicyExpression<int> durationSeconds, PolicyExpression<bool>? cacheResponse = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IOutbound CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IOutbound CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IOutbound Choose(Action<IChooseActions<IOutbound>> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IOutbound IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IOutbound Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/wait-policy</summary>
    IOutbound Wait(Action<IWaitActions<IOutbound>> actions, PolicyExpression<string>? waitFor = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IOutbound LimitConcurrency(PolicyExpression<string> key, int maxCount, Action<IOutbound> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IOutbound SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-service-bus-message-policy</summary>
    IOutbound SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties = null, PolicyExpression<string>? queueName = null, PolicyExpression<string>? topicName = null, PolicyExpression<string>? clientId = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-request-policy</summary>
    IOutbound SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IOutbound EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IOutbound LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/trace-policy</summary>
    IOutbound Trace(string source, PolicyExpression<string> message, TraceSeverity severity = TraceSeverity.Verbose, string? metadataName = null, string? metadataValue = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IOutbound FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IOutbound JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IOutbound MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy</summary>
    IOutbound RedirectContentUrls();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IOutbound ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy</summary>
    IOutbound SetBody(PolicyExpression<string> body);
    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-body-policy#using-liquid-templates-with-set-body</summary>
    IOutbound SetBody(LiquidTemplate body);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IOutbound SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IOutbound SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IOutbound SetVariable(string name, PolicyExpression<string> value);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy</summary>
    IOutbound XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xsl-transform-policy</summary>
    IOutbound XslTransform(string xslt, Action<IXslTransformParameters>? parameters = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-content-policy</summary>
    IOutbound ValidateContent(PolicyExpression<string> unspecifiedContentTypeAction, PolicyExpression<int> maxSizeBytes, PolicyExpression<string> sizeExceedAction, PolicyVariable? errorsVariableName = null, Action<IValidateContentActions>? validateActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-headers-policy</summary>
    IOutbound ValidateHeaders(PolicyExpression<string> specifiedHeaderAction, PolicyExpression<string> unspecifiedHeaderAction, PolicyVariable? errorsVariableName = null, Action<IValidateHeaderActions>? headers = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-status-code-policy</summary>
    IOutbound ValidateStatusCode(PolicyExpression<string> unspecifiedStatusCodeAction, PolicyVariable? errorVariableName = null, Action<IValidateStatusCodes>? statusCodes = null);
}

public interface IOnError
{
    IOnError Base();

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy</summary>
    IOnError CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue = null, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy</summary>
    IOnError CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy</summary>
    IOnError CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/choose-policy</summary>
    IOnError Choose(Action<IChooseActions<IOnError>> choose);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy</summary>
    IOnError IncludeFragment(string fragmentId);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/retry-policy</summary>
    IOnError Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds = null, int? deltaSeconds = null, PolicyExpression<bool>? firstFastRetry = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy</summary>
    IOnError LimitConcurrency(PolicyExpression<string> key, int maxCount, Action<IOnError> actions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy</summary>
    IOnError SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-service-bus-message-policy</summary>
    IOnError SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties = null, PolicyExpression<string>? queueName = null, PolicyExpression<string>? topicName = null, PolicyExpression<string>? clientId = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/send-request-policy</summary>
    IOnError SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode = null, PolicyExpression<int>? timeoutSeconds = null, bool? ignoreError = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy</summary>
    IOnError EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy</summary>
    IOnError LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy</summary>
    IOnError FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy</summary>
    IOnError JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, bool? parseDate = null, PolicyExpression<string>? namespaceSeparator = null, PolicyExpression<string>? namespacePrefix = null, PolicyExpression<string>? attributeBlockName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy</summary>
    IOnError MockResponse(int? statusCode = null, string? contentType = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/return-response-policy</summary>
    IOnError ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-header-policy</summary>
    IOnError SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction = null, Action<ISetHeaderValue>? values = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-method-policy</summary>
    IOnError SetMethod(PolicyExpression<string> method);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-status-policy</summary>
    IOnError SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy</summary>
    IOnError SetVariable(string name, PolicyExpression<string> value);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy</summary>
    IOnError XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader = null, PolicyExpression<bool>? alwaysArrayChildElements = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-content-policy</summary>
    IOnError ValidateContent(PolicyExpression<string> unspecifiedContentTypeAction, PolicyExpression<int> maxSizeBytes, PolicyExpression<string> sizeExceedAction, PolicyVariable? errorsVariableName = null, Action<IValidateContentActions>? validateActions = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-headers-policy</summary>
    IOnError ValidateHeaders(PolicyExpression<string> specifiedHeaderAction, PolicyExpression<string> unspecifiedHeaderAction, PolicyVariable? errorsVariableName = null, Action<IValidateHeaderActions>? headers = null);

    /// <summary>https://learn.microsoft.com/en-us/azure/api-management/validate-status-code-policy</summary>
    IOnError ValidateStatusCode(PolicyExpression<string> unspecifiedStatusCodeAction, PolicyVariable? errorVariableName = null, Action<IValidateStatusCodes>? statusCodes = null);

}
