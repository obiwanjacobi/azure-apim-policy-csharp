namespace Jacobi.Azure.ApiManagement.Policy;

internal interface IPolicyFragment : IAuthentication, ICaching, IControl, ICrossDomain, /*IGraphQL,*/
    IIngress, IIntegration, ILlm, ILogging, IRouting, ITransformation, IValidation
{ }

public abstract class PolicyFragment : PolicyDocumentBase,
    IPolicyFragment
{
    IPolicyFragment IAuthentication.AuthenticationBasic(PolicyExpression<string> username, PolicyExpression<string> password)
    {
        AuthenticationBasic(username, password);
        return this;
    }

    IPolicyFragment IAuthentication.AuthenticationCertificate(PolicyExpression<string> thumbprint, PolicyExpression<string> certificate, PolicyExpression<string>? body, PolicyExpression<string>? password)
    {
        AuthenticationCertificate(thumbprint, certificate, body, password);
        return this;
    }

    IPolicyFragment IAuthentication.AuthenticationManagedIdentity(PolicyExpression<string> resource, string? clientId, PolicyVariable? outputTokenVariableName, bool? ignoreError)
    {
        AuthenticationManagedIdentity(resource, clientId, outputTokenVariableName, ignoreError);
        return this;
    }

    IPolicyFragment IAuthentication.CheckHeader(PolicyExpression<string> name, PolicyExpression<int> failedCheckHttpCode, PolicyExpression<string> failedCheckErrorMessage, PolicyExpression<bool> ignoreCase, Action<ICheckHeaderValues>? values)
    {
        CheckHeader(name, failedCheckHttpCode, failedCheckErrorMessage, ignoreCase, values);
        return this;
    }

    IPolicyFragment IAuthentication.GetAuthorizationContext(PolicyExpression<string> providerId, PolicyExpression<string> authorizationId, PolicyVariable contextVariableName, PolicyExpression<string>? identity, PolicyExpression<bool>? ignoreError)
    {
        GetAuthorizationContext(providerId, authorizationId, contextVariableName, identity, ignoreError);
        return this;
    }

    IPolicyFragment IAuthentication.IpFilter(PolicyExpression<string> action, Action<IIpFilterAddress> address)
    {
        IpFilter(action, address);
        return this;
    }

    IPolicyFragment IAuthentication.ValidateAzureAdToken(PolicyExpression<string> tenantIdOrUrl, PolicyExpression<string>? headerName, PolicyExpression<string>? queryParameterName, PolicyExpression<string>? tokenValue, string? authenticationEndpoint, PolicyExpression<int>? failedValidationHttpCode, PolicyExpression<string>? failedValidationErrorMessage, PolicyVariable? outputTokenVariableName, Action<IValidateAzureAdTokenActions>? validationActions)
    {
        ValidateAzureAdToken(tenantIdOrUrl, headerName, queryParameterName, tokenValue, authenticationEndpoint, failedValidationHttpCode, failedValidationErrorMessage, outputTokenVariableName, validationActions);
        return this;
    }

    IPolicyFragment IAuthentication.ValidateClientCertificate(bool? validateRevocation, bool? validateTrust, bool? validateNotBefore, bool? validateNotAfter, bool? ignoreError, Action<IValidateClientCertificateIdentities>? identities)
    {
        ValidateClientCertificate(validateRevocation, validateTrust, validateNotBefore, validateNotAfter, ignoreError, identities);
        return this;
    }

    IPolicyFragment IAuthentication.ValidateJwt(PolicyExpression<string>? headerName, PolicyExpression<string>? queryParameterName, PolicyExpression<string>? tokenValue, PolicyExpression<int>? failedValidationHttpCode, PolicyExpression<string>? failedValidationErrorMessage, PolicyExpression<bool>? requireExpirationTime, PolicyExpression<string>? requireScheme, PolicyExpression<bool>? requireSignedTokens, PolicyExpression<int>? clockSkewSeconds, PolicyVariable? outputTokenVariableName, Action<IValidateJwtActions>? jwtActions)
    {
        ValidateJwt(headerName, queryParameterName, tokenValue, failedValidationHttpCode, failedValidationErrorMessage, requireExpirationTime, requireScheme, requireSignedTokens, clockSkewSeconds, outputTokenVariableName, jwtActions);
        return this;
    }

    IPolicyFragment ICaching.CacheLookup(PolicyExpression<bool> varyByDeveloper, PolicyExpression<bool> varyByDeveloperGroups, PolicyExpression<bool>? allowPrivateResponseCaching, CacheType? cacheType, PolicyExpression<string>? downstreamCacheType, PolicyExpression<bool>? mustRevalidate, Action<ICacheLookupVaryBy>? varyBy)
    {
        CacheLookup(varyByDeveloper, varyByDeveloperGroups, allowPrivateResponseCaching, cacheType, downstreamCacheType, mustRevalidate, varyBy);
        return this;
    }

    IPolicyFragment ICaching.CacheLookupValue(string variableName, PolicyExpression<string> key, PolicyExpression<string>? defaultValue, CacheType? cacheType)
    {
        CacheLookupValue(variableName, key, defaultValue, cacheType);
        return this;
    }

    IPolicyFragment ICaching.CacheStore(PolicyExpression<int> durationSeconds, PolicyExpression<bool>? cacheResponse)
    {
        CacheStore(durationSeconds, cacheResponse);
        return this;
    }

    IPolicyFragment ICaching.CacheStoreValue(PolicyExpression<int> durationSeconds, PolicyExpression<string> key, PolicyExpression<string> value, CacheType? cacheType)
    {
        CacheStoreValue(durationSeconds, key, value, cacheType);
        return this;
    }

    IPolicyFragment ICaching.CacheRemoveValue(PolicyExpression<string> key, CacheType? cacheType)
    {
        CacheRemoveValue(key, cacheType);
        return this;
    }

    IPolicyFragment IControl.Choose(Action<IChooseActions<IPolicyFragment>> choose)
    {
        Choose(this, choose);
        return this;
    }

    IPolicyFragment IControl.IncludeFragment(string fragmentId)
    {
        IncludeFragment(fragmentId);
        return this;
    }

    IPolicyFragment IControl.Retry(PolicyExpression<string> condition, PolicyExpression<int> numberOfRetries, PolicyExpression<int> intervalSeconds, PolicyExpression<int>? maxIntervalSeconds, int? deltaSeconds, PolicyExpression<bool>? firstFastRetry)
    {
        Retry(condition, numberOfRetries, intervalSeconds, maxIntervalSeconds, deltaSeconds, firstFastRetry);
        return this;
    }

    IPolicyFragment IControl.Wait(Action<IWaitActions<IPolicyFragment>> actions, PolicyExpression<string>? waitFor)
    {
        Wait(this, actions, waitFor);
        return this;
    }

    IPolicyFragment ICrossDomain.Cors(Action<ICorsActions> cors, bool? allowCredentials, bool? terminateUnmatchedRequests)
    {
        Cors(cors, allowCredentials, terminateUnmatchedRequests);
        return this;
    }

    IPolicyFragment ICrossDomain.CrossDomain(Action<ICrossDomainActions> actions, CrossDomainPolicies? permittedCrossDomainPolicies)
    {
        CrossDomain(actions, permittedCrossDomainPolicies);
        return this;
    }

    IPolicyFragment IIngress.LimitConcurrency(PolicyExpression<string> key, int maxCount, Action<IPolicyFragment> actions)
    {
        Writer.LimitConcurrency(key, maxCount.ToString(), () => actions(this));
        return this;
    }

    IPolicyFragment IIngress.Quota(int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, Action<IQuotaApi> apis)
    {
        Quota(numberOfCalls, bandwidthKB, renewalPeriodSeconds, apis);
        return this;
    }

    IPolicyFragment IIngress.QuotaByKey(PolicyExpression<string> counterKey, int numberOfCalls, int bandwidthKB, int renewalPeriodSeconds, PolicyExpression<int>? incrementCount, PolicyExpression<string>? incrementCondition, DateTime? firstPeriodStart)
    {
        QuotaByKey(counterKey, numberOfCalls, bandwidthKB, renewalPeriodSeconds, incrementCount, incrementCondition, firstPeriodStart);
        return this;
    }

    IPolicyFragment IIngress.RateLimit(int numberOfCalls, int renewalPeriodSeconds, PolicyVariable? retryAfterVariableName, string? retryAfterHeaderName, PolicyVariable? remainingCallsVariableName, string? remainingCallsHeaderName, string? totalCallsHeaderName, Action<IRateLimitApi>? apis)
    {
        RateLimit(numberOfCalls, renewalPeriodSeconds, retryAfterVariableName, retryAfterHeaderName, remainingCallsVariableName, remainingCallsHeaderName, totalCallsHeaderName, apis);
        return this;
    }

    IPolicyFragment IIngress.RateLimitByKey(PolicyExpression<string> counterKey, PolicyExpression<int> numberOfCalls, PolicyExpression<int> renewalPeriodSeconds, PolicyExpression<int>? incrementCount, PolicyExpression<string>? incrementCondition, PolicyVariable? retryAfterVariableName, string? retryAfterHeaderName, PolicyVariable? remainingCallsVariableName, string? remainingCallsHeaderName, string? totalCallsHeaderName)
    {
        RateLimitByKey(counterKey, numberOfCalls, renewalPeriodSeconds, incrementCount, incrementCondition, retryAfterVariableName, retryAfterHeaderName, remainingCallsVariableName, remainingCallsHeaderName, totalCallsHeaderName);
        return this;
    }

    IPolicyFragment IIntegration.PublishToDapr(PolicyExpression<string> message, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        PublishToDapr(message, topic, pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }

    IPolicyFragment IIntegration.PublishToDapr(LiquidTemplate template, PolicyExpression<string> topic, PolicyExpression<string>? pubSubName, PolicyVariable? responseVariableName, PolicyExpression<int>? timeoutSeconds, string? contentType, bool? ignoreError)
    {
        PublishToDapr(template, topic, pubSubName, responseVariableName, timeoutSeconds, contentType, ignoreError);
        return this;
    }

    IPolicyFragment IIntegration.SendOneWayRequest(Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds)
    {
        SendOneWayRequest(request, mode, timeoutSeconds);
        return this;
    }

    IPolicyFragment IIntegration.SendServiceBusMessage(PolicyExpression<string> @namespace, PolicyExpression<string> message, Action<ISendServiceBusMessageProperties>? messageProperties, PolicyExpression<string>? queueName, PolicyExpression<string>? topicName, PolicyExpression<string>? clientId)
    {
        SendServiceBusMessage(@namespace, message, messageProperties, queueName, topicName, clientId);
        return this;
    }

    IPolicyFragment IIntegration.SendRequest(PolicyExpression<string> responseVariableName, Action<ISendRequestActions> request, PolicyExpression<string>? mode, PolicyExpression<int>? timeoutSeconds, bool? ignoreError)
    {
        SendRequest(responseVariableName, request, mode, timeoutSeconds, ignoreError);
        return this;
    }

    IPolicyFragment IIntegration.SetBackendService(PolicyExpression<string> daprAppId, PolicyExpression<string> daprMethod, PolicyExpression<string>? daprNamespace)
    {
        SetBackendService(daprAppId, daprMethod, daprNamespace);
        return this;
    }

    IPolicyFragment ILogging.EmitMetric(string name, string? @namespace, string? value, Action<IEmitMetricDimensions> dimensions)
    {
        EmitMetric(name, @namespace, value, dimensions);
        return this;
    }

    IPolicyFragment ILogging.LogToEventHub(string loggerId, string? partitionId, string? partitionKey, PolicyExpression<string> message)
    {
        LogToEventHub(loggerId, partitionId, partitionKey, message);
        return this;
    }

    IPolicyFragment ILogging.Trace(string source, PolicyExpression<string> message, TraceSeverity severity, string? metadataName, string? metadataValue)
    {
        Trace(source, message, severity, metadataName, metadataValue);
        return this;
    }

    IPolicyFragment IRouting.ForwardRequest(HttpVersion? httpVersion, PolicyExpression<int>? timeoutSeconds, PolicyExpression<int>? timeoutMilliseconds, PolicyExpression<int>? continueTimeout, bool? followRedirects, bool? bufferRequestBody, bool? bufferResponse, bool? failOnErrorStatusCode)
    {
        ForwardRequest(httpVersion, timeoutSeconds, timeoutMilliseconds, continueTimeout, followRedirects, bufferRequestBody, bufferResponse, failOnErrorStatusCode);
        return this;
    }

    IPolicyFragment IRouting.Proxy(PolicyExpression<string> url, PolicyExpression<string>? username, PolicyExpression<string>? password)
    {
        Proxy(url, username, password);
        return this;
    }

    IPolicyFragment IRouting.SetBackendService(PolicyExpression<string> baseUrl)
    {
        SetBackendService(baseUrl);
        return this;
    }

    IPolicyFragment IRouting.SetBackendService(PolicyExpression<string> backendId, PolicyExpression<string>? sfResolveCondition, PolicyExpression<string>? sfServiceInstanceName, PolicyExpression<string>? sfPartitionKey, PolicyExpression<string>? sfListenerName)
    {
        SetBackendService(backendId, sfResolveCondition, sfServiceInstanceName, sfPartitionKey, sfListenerName);
        return this;
    }

    IPolicyFragment ITransformation.FindAndReplace(PolicyExpression<string> from, PolicyExpression<string> to)
    {
        FindAndReplace(from, to);
        return this;
    }

    IPolicyFragment ITransformation.JsonToXml(PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, bool? parseDate, PolicyExpression<string>? namespaceSeparator, PolicyExpression<string>? namespacePrefix, PolicyExpression<string>? attributeBlockName)
    {
        JsonToXml(apply, considerAcceptHeader, parseDate, namespaceSeparator, namespacePrefix, attributeBlockName);
        return this;
    }

    IPolicyFragment ITransformation.MockResponse(int? statusCode, string? contentType)
    {
        MockResponse(statusCode, contentType);
        return this;
    }

    IPolicyFragment ITransformation.RedirectContentUrls()
    {
        RedirectContentUrls();
        return this;
    }

    IPolicyFragment ITransformation.ReturnResponse(Action<IReturnResponseActions> response, PolicyVariable? responseVariableName)
    {
        ReturnResponse(response, responseVariableName);
        return this;
    }

    IPolicyFragment ITransformation.RewriteUri(PolicyExpression<string> template, bool? copyUnmatchedParams)
    {
        RewriteUri(template, copyUnmatchedParams);
        return this;
    }

    IPolicyFragment ITransformation.SetBody(PolicyExpression<string> body)
    {
        SetBody(body);
        return this;
    }

    IPolicyFragment ITransformation.SetBody(LiquidTemplate body)
    {
        SetBody(body);
        return this;
    }

    IPolicyFragment ITransformation.SetHeader(PolicyExpression<string> name, PolicyExpression<string>? existsAction, Action<ISetHeaderValue>? values)
    {
        SetHeader(name, existsAction, values);
        return this;
    }

    IPolicyFragment ITransformation.SetMethod(PolicyExpression<string> method)
    {
        SetMethod(method);
        return this;
    }

    IPolicyFragment ITransformation.SetStatus(PolicyExpression<int> statusCode, PolicyExpression<string> reason)
    {
        SetStatus(statusCode, reason);
        return this;
    }

    IPolicyFragment ITransformation.SetQueryParameter(PolicyExpression<string> name, Action<ISetQueryParameterValue> values, PolicyExpression<string>? existsAction)
    {
        SetQueryParameter(name, values, existsAction);
        return this;
    }

    IPolicyFragment ITransformation.SetVariable(string name, PolicyExpression<string> value)
    {
        SetVariable(name, value);
        return this;
    }

    IPolicyFragment ITransformation.XmlToJson(PolicyExpression<string> kind, PolicyExpression<string> apply, PolicyExpression<bool>? considerAcceptHeader, PolicyExpression<bool>? alwaysArrayChildElements)
    {
        XmlToJson(kind, apply, considerAcceptHeader, alwaysArrayChildElements);
        return this;
    }

    IPolicyFragment ITransformation.XslTransform(string xslt, Action<IXslTransformParameters>? parameters)
    {
        XslTransform(xslt, parameters);
        return this;
    }

    IPolicyFragment IValidation.ValidateContent(PolicyExpression<string> unspecifiedContentTypeAction, PolicyExpression<int> maxSizeBytes, PolicyExpression<string> sizeExceedAction, PolicyVariable? errorsVariableName, Action<IValidateContentActions>? validateActions)
    {
        ValidateContent(unspecifiedContentTypeAction, maxSizeBytes, sizeExceedAction, errorsVariableName, validateActions);
        return this;
    }

    IPolicyFragment IValidation.ValidateHeaders(PolicyExpression<string> specifiedHeaderAction, PolicyExpression<string> unspecifiedHeaderAction, PolicyVariable? errorsVariableName, Action<IValidateHeaderActions>? headers)
    {
        ValidateHeaders(specifiedHeaderAction, unspecifiedHeaderAction, errorsVariableName, headers);
        return this;
    }

    IPolicyFragment IValidation.ValidateODataRequest(PolicyVariable? errorVariableName, string? defaultODataVersion, string? minODataVersion, string? maxODataVersion, int? maxSizeBytes)
    {
        ValidateODataRequest(errorVariableName, defaultODataVersion, minODataVersion, maxODataVersion, maxSizeBytes);
        return this;
    }

    IPolicyFragment IValidation.ValidateParameters(PolicyExpression<string> specifiedParameterAction, PolicyExpression<string> unspecifiedParameterAction, PolicyVariable? errorVariableName, Action<IValidateParameterActions>? parameterActions)
    {
        ValidateParameters(specifiedParameterAction, unspecifiedParameterAction, errorVariableName, parameterActions);
        return this;
    }

    IPolicyFragment IValidation.ValidateStatusCode(PolicyExpression<string> unspecifiedStatusCodeAction, PolicyVariable? errorVariableName, Action<IValidateStatusCodes>? statusCodes)
    {
        ValidateStatusCode(unspecifiedStatusCodeAction, errorVariableName, statusCodes);
        return this;
    }
}
