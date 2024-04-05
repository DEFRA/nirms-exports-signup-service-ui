using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[ExcludeFromCodeCoverage]
[FeatureGate(FeatureFlags.SelfServe)]
public class AddEstablishmentHoldingModel : BasePageModel<AddEstablishmentHoldingModel>
{
    [BindProperty]
    public Guid RegistrationId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    public string Country { get; set; } = default!;
    public string CountryText { get; set; } = "Add a place of dispatch";

    public AddEstablishmentHoldingModel(
    ILogger<AddEstablishmentHoldingModel> logger,
    IEstablishmentService establishmentService,
    ITraderService traderService) : base(logger, traderService, establishmentService)
    { }

    public async Task<IActionResult> OnGet(Guid id, string country)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AddEstablishmentHoldingModel), nameof(OnGet));

        OrgId = id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        RegistrationId = tradeParty!.Id;

        Country = country;

        if(Country == "NI")
        {
            CountryText = "Add a place of destination";
        }

        return Page();
    }

    public IActionResult OnPostSubmit()
    {
        return RedirectToPage(
                Routes.Pages.Path.SelfServeDashboardPath,
                new { id = OrgId });
    }
}
