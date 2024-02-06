using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class ConfirmEstablishmentDetailsModel : BasePageModel<ConfirmEstablishmentDetailsModel>
{
    #region UI Models
    [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDto? Location { get; set; } = new LogisticsLocationDto();
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid EstablishmentId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    [BindProperty]
    public string Country { get; set; } = default!;
    public string? BusinessName { get; set; }
    #endregion

    public ConfirmEstablishmentDetailsModel(
        ILogger<ConfirmEstablishmentDetailsModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string country)
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        OrgId = id;
        EstablishmentId = locationId;
        Country = country;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);

        if (tradeParty != null)
        {
            TradePartyId = tradeParty.Id;
            BusinessName = tradeParty.PracticeName;
        }

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (Country == "NI")
        {
            ContentHeading = "Places of destination";
            ContentText = "destination";
        }
        else
        {
            ContentHeading = "Places of dispatch";
            ContentText = "dispatch";
        }

        if (TradePartyId != Guid.Empty && EstablishmentId != Guid.Empty)
        {
            Location = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);
            Email = Location?.Email;
        }

        return Page();
    }

    public IActionResult OnPostSubmitAsync()
    {
        _logger.LogInformation("Self serve confirm establishment OnPostSubmit");

        return RedirectToPage(
            Routes.Pages.Path.SelfServeRegulationsPath,
            new { id = OrgId, locationId = EstablishmentId, country = Country });
    }

    public async Task<IActionResult> OnGetRemoveEstablishment(Guid orgId, Guid tradePartyId, Guid establishmentId, string country)
    {
        var logisticsLocation = await _establishmentService.GetEstablishmentByIdAsync(establishmentId);
        logisticsLocation!.IsRemoved = true;
        await _establishmentService.UpdateEstablishmentDetailsAsync(logisticsLocation);
        return RedirectToPage(Routes.Pages.Path.SelfServeDashboardPath, new { id = orgId});
    }

    public IActionResult OnGetChangeEstablishmentAddress(Guid orgId, Guid establishmentId, string country)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentNameAndAddressPath,
            new { id = orgId, establishmentId, country });
    }

    public IActionResult OnGetChangeEmail(Guid orgId, Guid establishmentId, string country)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentContactEmailPath,
            new { id = orgId, locationId = establishmentId, country });
    }
}
