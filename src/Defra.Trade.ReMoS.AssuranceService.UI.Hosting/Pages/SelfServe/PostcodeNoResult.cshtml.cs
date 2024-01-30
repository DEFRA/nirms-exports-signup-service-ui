using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class PostcodeNoResultModel : BasePageModel<PostcodeNoResultModel>
{
    #region UI models
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public string Postcode { get; set; } = string.Empty;
    [BindProperty]
    public string Country { get; set; } = default!;
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? ContentCountry { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    #endregion

    public PostcodeNoResultModel(ITraderService traderService) : base(traderService)
    { }

    public async Task<IActionResult> OnGet(Guid id, string country, string postcode)
    {
        OrgId = id;
        Country = country;
        Postcode = postcode;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        await GetBusinessNameAsync();

        if (Country == "NI")
        {
            ContentCountry = "Northern Ireland";
            ContentHeading = "Add a place of destination";
            ContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
        }
        else
        {
            ContentCountry = "England, Scotland and Wales";
            ContentHeading = "Add a place of dispatch";
            ContentText = "The locations which are part of your business that consignments to Northern Ireland will depart from under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
        }

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        return Page();
    }

    private async Task GetBusinessNameAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null)
        {
            BusinessName = tradeParty.PracticeName;
        }
    }
}
