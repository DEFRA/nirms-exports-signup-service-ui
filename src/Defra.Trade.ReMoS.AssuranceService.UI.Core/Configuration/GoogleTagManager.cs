using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class GoogleTagManager
    {
        public string ContainerId { get; set; } = string.Empty;
        public string MeasurementId { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }
}
