using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class PostcodeNoResultModel : PageModel
{
    #region UI models
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public string Postcode { get; set; } = string.Empty;
    [BindProperty]
    public string NI_GBFlag { get; set; } = string.Empty;
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? ContentCountry { get; set; } = string.Empty;
    #endregion

    private readonly ITraderService _traderService;

    public PostcodeNoResultModel(ITraderService traderService)
    {
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService)); 
    }

    public IActionResult OnGet(Guid id, string NI_GBFlag, string postcode)
    {
        TradePartyId = id;
        this.NI_GBFlag = NI_GBFlag;
        Postcode = postcode;

        if (_traderService.IsTradePartySignedUp(TradePartyId).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        if (NI_GBFlag == "NI")
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

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        return Page();
    }
}
