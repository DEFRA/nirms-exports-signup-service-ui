using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

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
    public string NI_GBFlag { get; set; } = string.Empty;
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? ContentCountry { get; set; } = string.Empty;
    #endregion

    public PostcodeNoResultModel(ITraderService traderService, ILogger<PostcodeNoResultModel> logger) : base(traderService, logger)
    {}

    public async Task<IActionResult> OnGet(Guid id, string NI_GBFlag, string postcode)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(PostcodeNoResultModel), nameof(OnGet));

        OrgId = id;
        this.NI_GBFlag = NI_GBFlag;
        Postcode = postcode;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (_traderService.IsTradePartySignedUp(tradeParty))
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

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        return Page();
    }
}
