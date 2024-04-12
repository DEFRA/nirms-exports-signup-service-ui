using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class EhcoIntegration
    {
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}
