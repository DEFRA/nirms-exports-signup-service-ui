using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
[ExcludeFromCodeCoverage]
public class EstablishmentAddedModel : PageModel
{
    public void OnGet()
    {
        // To be implemented in next story
    }
}
