using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[ExcludeFromCodeCoverage]
public class AddEstablishmentHoldingModel : BasePageModel<AddEstablishmentHoldingModel>
{
    [BindProperty]
    public Guid RegistrationId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    public string Country { get; set; } = default!;
    public string CountryText { get; set; } = "Add a place of dispatch";

    public IActionResult OnGet(Guid id, string country)
    {
        OrgId = id;
        RegistrationId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;
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
