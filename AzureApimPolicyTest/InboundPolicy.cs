using AzureApimPolicyGen;

namespace AzureApimPolicyTest;

internal class CachePolicy : PolicyDocument
{
    protected override void Inbound()
    {
        Authentication.Basic("username", "password");
        Cache.Lookup(false, false, varyBy: varyBy =>
        {
            varyBy.Header("Content-Type");
            varyBy.QueryParam("page");
        });
        base.Inbound();
    }
}
