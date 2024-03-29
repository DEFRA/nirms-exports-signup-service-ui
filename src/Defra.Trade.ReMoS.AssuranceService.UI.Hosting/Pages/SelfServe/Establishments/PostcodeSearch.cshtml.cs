using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class PostcodeSearchModel : BasePageModel<PostcodeSearchModel>
{
    #region UI Models

    [BindProperty]
    [RegularExpression(@"^([Gg][Ii][Rr] 0[Aa]{2}|([A-Za-z][0-9]{1,2}|[A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2}|[A-Za-z][0-9][A-Za-z]|[A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]) ?[0-9][A-Za-z]{2})$", ErrorMessage = "Enter a real postcode")]
    [StringLengthMaximum(100, ErrorMessage = "Postcode must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a postcode.")]
    public string? Postcode { get; set; } = string.Empty;

    public string? BusinessName { get; set; }

    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? ContextHint { get; set; } = string.Empty;

    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    
    [BindProperty]
    public Guid OrgId { get; set; }

    #endregion UI Models


    public PostcodeSearchModel(ILogger<PostcodeSearchModel> logger, ITraderService traderService) : base(logger, traderService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
    {
        _logger.LogTrace("Self serve Establishment postcode search on get");
        OrgId = id;
        this.NI_GBFlag = NI_GBFlag;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        BusinessName = await _traderService.GetBusinessNameAsync(TradePartyId);

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (NI_GBFlag == "NI")
        {
            ContextHint = "If your place of destination belongs to a different business";
            ContentHeading = "Add a place of destination";
            ContentText = "where consignments will go after the port of entry under the scheme";
        }
        else
        {
            ContextHint = "If your place of dispatch belongs to a different business";
            ContentHeading = "Add a place of dispatch";
            ContentText = "from which consignments to Northern Ireland will depart under the scheme";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId, NI_GBFlag!);
        }

        if (Postcode!.ToUpper().StartsWith("BT") && (NI_GBFlag == "GB"))
        {
            var baseError = "Enter a postcode in England, Scotland or Wales";
            ModelState.AddModelError(nameof(Postcode), baseError);
            return await OnGetAsync(OrgId, NI_GBFlag);
        }
        if (!Postcode!.ToUpper().StartsWith("BT") && (NI_GBFlag == "NI"))
        {
            var baseError = "Enter a postcode in Northern Ireland";
            ModelState.AddModelError(nameof(Postcode), baseError);
            return await OnGetAsync(OrgId, NI_GBFlag);
        }

        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentPostcodeResultPath,
            new { id = OrgId, postcode = Postcode, NI_GBFlag });
    }
}
