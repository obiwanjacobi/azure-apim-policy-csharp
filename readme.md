# Azure APIM Policy CSharp

This library allows you to generate Azure API Management Policy XML files based on C# class implementations.

## Usage

You implement a class for each Policy Xml file you whish to generate. The method and parameter names are kept the same as the Azure API Management Policy documentation to make it very easy to know what stuff does. See also the list of implemented policies below.

```csharp
public sealed class MyPolicy : Jacobi.Azure.ApiManagement.Policy.PolicyDocument
{
    protected override void Inbound(IInbound inbound)
    {
      inbound
        .CheckHeader(...)
        .IpFilter(...)
        // other policies
        ;

        base.Inbound(inbound); // generates the <Base/> implicitly.
    }

    protected override void Backend(IBackend backend)
    {
        // add your policies here
        base.Backend(backend);
    }

    protected override void Outbound(IOutbound outbound)
    {
        // add your policies here
        base.Outbound(outbound);
    }

    protected override void OnError(IOnError onError)
    {
        // add your policies here
        base.OnError(onError);
    }
}
```

Simularly for a policy fragment:

```csharp
public sealed class MyFragment : Jacobi.Azure.ApiManagement.Policy.PolicyFragment
{
    protected override void Fragment(IPolicyFragment fragment)
    {
      fragment
        .CheckHeader(...)
        .IpFilter(...)
        // other policies
        ;
      
      // no need to call the base
    }
}
```

When the project is built (successfully) a Policy XML file will be generated for each class - with the same file name (.xml) as the class.

Note that the resulting dotnet assembly is not used in anyway.

## Policy Expressions

Any values that can be an expression (either code or a literal value) is represented by the `PolicyExpression` class.

There are two implementation of this class:

- `PolicyExpression<T>` that is used in the interface method declarations to indicate whet data type is ultimately expected for the value. Although at this point, the return-type of any code is not verified, some validation takes place - it servers more as documentation.
- `PolicyExpression` is the type you typically use when constructing expressions.

An example of specifying a code expression using the `PolicyExpression.FromCode` would be:

```csharp
public sealed class MyPolicy : Jacobi.Azure.ApiManagement.Policy.PolicyDocument
{
    protected override void Inbound(IInbound inbound)
    {
      inbound
        .Choose(choose =>
            choose.When(PolicyExpression.FromCode("""Context.Variables.GetValueOrDefault<bool>("myvar", true)"""),
                actions => actions.SetBody(LiquidTemplate.From(""" body """))))
        base.Inbound(inbound);
    }
}
```

Note the use of the `LiquidTemplate` struct. Support for Liquid-templates is very rudementary and serves basically as a way to distinguish between `PolicyExpression` and `LiquidTemplate` strings.

## Policies Implemented

- [x] [`authentication-basic`](https://learn.microsoft.com/en-us/azure/api-management/authentication-basic-policy)
- [x] [`authentication-certificate`](https://learn.microsoft.com/en-us/azure/api-management/authentication-certificate-policy)
- [x] [`authentication-managed-identity`](https://learn.microsoft.com/en-us/azure/api-management/authentication-managed-identity-policy)
- [ ] [`azure-openai-emit-token-metric`](https://learn.microsoft.com/en-us/azure/api-management/azure-openai-emit-token-metric-policy)
- [ ] [`azure-openai-semantic-cache-lookup`](https://learn.microsoft.com/en-us/azure/api-management/azure-openai-semantic-cache-lookup-policy)
- [ ] [`azure-openai-semantic-cache-store`](https://learn.microsoft.com/en-us/azure/api-management/azure-openai-semantic-cache-store-policy)
- [ ] [`azure-openai-token-limit`](https://learn.microsoft.com/en-us/azure/api-management/azure-openai-token-limit-policy)
- [x] [`cache-lookup`](https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-policy)
- [x] [`cache-lookup-value`](https://learn.microsoft.com/en-us/azure/api-management/cache-lookup-value-policy)
- [x] [`cache-store`](https://learn.microsoft.com/en-us/azure/api-management/cache-store-policy)
- [x] [`cache-store-value`](https://learn.microsoft.com/en-us/azure/api-management/cache-store-value-policy)
- [x] [`cache-remove-value`](https://learn.microsoft.com/en-us/azure/api-management/cache-remove-value-policy)
- [x] [`check-header`](https://learn.microsoft.com/en-us/azure/api-management/check-header-policy)
- [x] [`choose`](https://learn.microsoft.com/en-us/azure/api-management/choose-policy)
- [x] [`cors`](https://learn.microsoft.com/en-us/azure/api-management/cors-policy)
- [ ] [`cosmosdb-data-source`](https://learn.microsoft.com/en-us/azure/api-management/cosmosdb-data-source-policy)
- [x] [`cross-domain`](https://learn.microsoft.com/en-us/azure/api-management/cross-domain-policy)
- [x] [`emit-metric`](https://learn.microsoft.com/en-us/azure/api-management/emit-metric-policy)
- [x] [`find-and-replace`](https://learn.microsoft.com/en-us/azure/api-management/find-and-replace-policy)
- [x] [`forward-request`](https://learn.microsoft.com/en-us/azure/api-management/forward-request-policy)
- [x] [`get-authorization-context`](https://learn.microsoft.com/en-us/azure/api-management/get-authorization-context-policy)
- [ ] [`http-data-source`](https://learn.microsoft.com/en-us/azure/api-management/http-data-source-policy)
- [x] [`include-fragment`](https://learn.microsoft.com/en-us/azure/api-management/include-fragment-policy)
- [ ] [`invoke-dapr-binding`](https://learn.microsoft.com/en-us/azure/api-management/invoke-dapr-binding-policy)
- [x] [`ip-filter`](https://learn.microsoft.com/en-us/azure/api-management/ip-filter-policy)
- [ ] [`jsonp`](https://learn.microsoft.com/en-us/azure/api-management/jsonp-policy)
- [x] [`json-to-xml`](https://learn.microsoft.com/en-us/azure/api-management/json-to-xml-policy)
- [x] [`limit-concurrency`](https://learn.microsoft.com/en-us/azure/api-management/limit-concurrency-policy)
- [ ] [`llm-content-safety`](https://learn.microsoft.com/en-us/azure/api-management/llm-content-safety-policy)
- [ ] [`llm-emit-token-metric`](https://learn.microsoft.com/en-us/azure/api-management/llm-emit-token-metric-policy)
- [ ] [`llm-semantic-cache-lookup`](https://learn.microsoft.com/en-us/azure/api-management/llm-semantic-cache-lookup-policy)
- [ ] [`llm-semantic-cache-store`](https://learn.microsoft.com/en-us/azure/api-management/llm-semantic-cache-store-policy)
- [ ] [`llm-token-limit`](https://learn.microsoft.com/en-us/azure/api-management/llm-token-limit-policy)
- [x] [`log-to-eventhub`](https://learn.microsoft.com/en-us/azure/api-management/log-to-eventhub-policy)
- [x] [`mock-response`](https://learn.microsoft.com/en-us/azure/api-management/mock-response-policy)
- [x] [`proxy`](https://learn.microsoft.com/en-us/azure/api-management/proxy-policy)
- [ ] [`publish-event`](https://learn.microsoft.com/en-us/azure/api-management/publish-event-policy)
- [x] [`publish-to-dapr`](https://learn.microsoft.com/en-us/azure/api-management/publish-to-dapr-policy)
- [x] [`quota`](https://learn.microsoft.com/en-us/azure/api-management/quota-policy)
- [x] [`quota-by-key`](https://learn.microsoft.com/en-us/azure/api-management/quota-by-key-policy)
- [x] [`rate-limit`](https://learn.microsoft.com/en-us/azure/api-management/rate-limit-policy)
- [x] [`rate-limit-by-key`](https://learn.microsoft.com/en-us/azure/api-management/rate-limit-by-key-policy)
- [x] [`redirect-content-urls`](https://learn.microsoft.com/en-us/azure/api-management/redirect-content-urls-policy)
- [x] [`retry`](https://learn.microsoft.com/en-us/azure/api-management/retry-policy)
- [x] [`return-response`](https://learn.microsoft.com/en-us/azure/api-management/return-response-policy)
- [x] [`rewrite-uri`](https://learn.microsoft.com/en-us/azure/api-management/rewrite-uri-policy)
- [x] [`send-one-way-request`](https://learn.microsoft.com/en-us/azure/api-management/send-one-way-request-policy)
- [x] [`send-service-bus-message`](https://learn.microsoft.com/en-us/azure/api-management/send-service-bus-message-policy)
- [x] [`send-request`](https://learn.microsoft.com/en-us/azure/api-management/send-request-policy)
- [x] [`set-backend-service`](https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-policy)
- [x] [`set-backend-service-dapr`](https://learn.microsoft.com/en-us/azure/api-management/set-backend-service-dapr-policy)
- [x] [`set-body`](https://learn.microsoft.com/en-us/azure/api-management/set-body-policy)
- [x] [`set-header`](https://learn.microsoft.com/en-us/azure/api-management/set-header-policy)
- [x] [`set-method`](https://learn.microsoft.com/en-us/azure/api-management/set-method-policy)
- [x] [`set-query-parameter`](https://learn.microsoft.com/en-us/azure/api-management/set-query-parameter-policy)
- [x] [`set-status`](https://learn.microsoft.com/en-us/azure/api-management/set-status-policy)
- [x] [`set-variable`](https://learn.microsoft.com/en-us/azure/api-management/set-variable-policy)
- [ ] [`sql-data-source`](https://learn.microsoft.com/en-us/azure/api-management/sql-data-source-policy)
- [x] [`trace`](https://learn.microsoft.com/en-us/azure/api-management/trace-policy)
- [x] [`validate-azure-ad-token`](https://learn.microsoft.com/en-us/azure/api-management/validate-azure-ad-token-policy)
- [x] [`validate-client-certificate`](https://learn.microsoft.com/en-us/azure/api-management/validate-client-certificate-policy)
- [x] [`validate-content`](https://learn.microsoft.com/en-us/azure/api-management/validate-content-policy)
- [ ] [`validate-graphql-request`](https://learn.microsoft.com/en-us/azure/api-management/validate-graphql-request-policy)
- [x] [`validate-headers`](https://learn.microsoft.com/en-us/azure/api-management/validate-headers-policy)
- [x] [`validate-jwt`](https://learn.microsoft.com/en-us/azure/api-management/validate-jwt-policy)
- [x] [`validate-odata-request`](https://learn.microsoft.com/en-us/azure/api-management/validate-odata-request-policy)
- [x] [`validate-parameters`](https://learn.microsoft.com/en-us/azure/api-management/validate-parameters-policy)
- [x] [`validate-status-code`](https://learn.microsoft.com/en-us/azure/api-management/validate-status-code-policy)
- [x] [`wait`](https://learn.microsoft.com/en-us/azure/api-management/wait-policy)
- [x] [`xml-to-json`](https://learn.microsoft.com/en-us/azure/api-management/xml-to-json-policy)
- [x] [`xsl-transform`](https://learn.microsoft.com/en-us/azure/api-management/xsl-transform-policy)


## Policy XML Files Output Folder

By default the generated policy XML files are written to the `PolicyXml` folder in the project root folder.
However this can be easily changed by specifying an override in your .csproj file.

```xml
<PropertyGroup>
  <PolicyXmlOutputFolder>$(TargetPath)\MyCustomPolicyXmlFolder</PolicyXmlOutputFolder>
</PropertyGroup>
```

Replacement tokens (macros) can be used to build a folder path to where the policy xml files will be written.
This example creates a new 'MyCustomPolicyXmlFolder' folder in the build-output folder next to the binaries.

---

## TODO

For transparency:

- [ ] Variables: check exists (variable-references in policies are typically created on demand)
- [ ] Variables: Typed - to check the 'structure' of variables
- [ ] PolicyExpression: TypeHint to document expected (result) type of literal or code expression
- [ ] CodeExpression: `{{named-value}}` in code. Compiler will fail.
- [ ] CodeExpression Compiler: `(string)Context.Variables["connectionId"]` error CS0201: Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement (suppressed for now)
- [ ] Int32: validate non-negative or use uint.
- [ ] Fragment scope: unclear what policies can (not) go in fragments
- [ ] XslTransform: represent the xslt as something else than a plain string?
- [ ] Keep track of the number of policy uses per document (for instance: cors can only be used once).
- [ ] 
