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
[BindProperties]
public class ContactEmailModel : BasePageModel<ContactEmailModel>
{
    #region UI Models
    [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDto? Location { get; set; } = new LogisticsLocationDto();
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string Country { get; set; } = default!;
    public string? BusinessName { get; set; }
    #endregion

    public ContactEmailModel(
        ILogger<ContactEmailModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
    {}

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

        if (country == "NI")
        {
            ContentHeading = "Add a place of destination";
        }
        else
        {
            ContentHeading = "Add a place of dispatch";
        }

        if (TradePartyId != Guid.Empty && EstablishmentId != Guid.Empty)
        {
            Location = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);
            Email = Location?.Email;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment contact email OnPostSubmit");

        if (!IsInputValid())
        {
            return await OnGetAsync(OrgId, EstablishmentId, Country ?? string.Empty);
        }

        await SaveEmailToApi();

        return RedirectToPage(
            Routes.Pages.Path.SelfServeConfirmEstablishmentDetailsPath, 
            new { id = OrgId, locationId = EstablishmentId,  Country});
    }

    public IActionResult OnGetChangeEstablishmentAddress(Guid orgId, Guid establishmentId, string country)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentNameAndAddressPath,
            new { id = orgId, establishmentId, country });
    }

    private async Task SaveEmailToApi()
    {
        Location = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);

        if (Location != null)
        {
            Location.Email = Email;
            await _establishmentService.UpdateEstablishmentDetailsAsync(Location);
        }
    }

    private bool IsInputValid()
    {
        if (Email != null && Email.Length > 100)
            ModelState.AddModelError(nameof(Email), "The email address cannot be longer than 100 characters");

        if (!ModelState.IsValid || ModelState.ErrorCount > 0)
            return false;

        return true;
    }
}
