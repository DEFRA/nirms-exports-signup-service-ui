using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
[ExcludeFromCodeCoverage]
public class EligibilityRegulationsModel : BasePageModel<EligibilityRegulationsModel>
{
    [BindProperty]
    public bool Confirmed { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string Country { get; set; } = default!;

    public EligibilityRegulationsModel(
        ILogger<EligibilityRegulationsModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}

    public IActionResult OnGetAsync(Guid id, Guid locationId, string country)
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        OrgId = id;
        EstablishmentId = locationId;
        Country = country;

        return Page();
    }

}
