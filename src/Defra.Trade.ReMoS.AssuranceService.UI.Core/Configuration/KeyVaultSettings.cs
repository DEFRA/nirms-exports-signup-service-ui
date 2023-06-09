using Defra.Trade.Common.Security.AzureKeyVault.Attributes;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;

public class KeyVaultSettings
{
    //public const string KeyVaultSecretsSettingsName = "KeyVaultSecretsSettings";

    //[SecretName("ApimInternalClientsSubscription")]
    //public string ApimInternalClientsSubscription { get; set; }

    //[SecretName("GCOsplace-ApiKey")]
    //public string GCOsplaceApiKey { get; set; }

    public string SubscriptionKey { get; set; }
}
