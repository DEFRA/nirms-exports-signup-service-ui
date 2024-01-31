using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class PostcodeSearchModel : BasePageModel<PostcodeSearchModel>
{
    #region UI Models

    [BindProperty]
    [RegularExpression(@"^([Gg][Ii][Rr] 0[Aa]{2}|([A-Za-z][0-9]{1,2}|[A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2}|[A-Za-z][0-9][A-Za-z]|[A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]) ?[0-9][A-Za-z]{2})$", ErrorMessage = "Enter a real postcode")]
    [StringLength(100, ErrorMessage = "Postcode must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a postcode.")]
    public string? Postcode { get; set; } = string.Empty;

    public string? BusinessName { get; set; }

    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? ContextHint { get; set; } = string.Empty;

    [BindProperty]
    public string Country { get; set; } = default!;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    
    [BindProperty]
    public Guid OrgId { get; set; }

    #endregion UI Models


    public PostcodeSearchModel(ILogger<PostcodeSearchModel> logger, ITraderService traderService) : base(logger, traderService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, string country)
    {
        _logger.LogTrace("Self serve Establishment postcode search on get");
        OrgId = id;
        Country = country;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        await GetBusinessNameAsync();

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (Country == "NI")
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
            return await OnGetAsync(OrgId, Country!);
        }

        if (Postcode!.ToUpper().StartsWith("BT") && CountryInGb(Country))
        {
            var baseError = "Enter a postcode in England, Scotland or Wales";
            ModelState.AddModelError(nameof(Postcode), baseError);
            return await OnGetAsync(OrgId, Country);
        }
        if (!Postcode!.ToUpper().StartsWith("BT") && (Country == "NI"))
        {
            var baseError = "Enter a postcode in Northern Ireland";
            ModelState.AddModelError(nameof(Postcode), baseError);
            return await OnGetAsync(OrgId, Country);
        }

        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentPostcodeResultPath,
            new { id = OrgId, postcode = Postcode, Country });
    }

    private static bool CountryInGb(string country)
    {
        if (country.ToUpper() == "ENGLAND" || country.ToUpper() == "WALES" || country.ToUpper() == "SCOTLAND")
        {
            return true;
        }
        return false;
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
