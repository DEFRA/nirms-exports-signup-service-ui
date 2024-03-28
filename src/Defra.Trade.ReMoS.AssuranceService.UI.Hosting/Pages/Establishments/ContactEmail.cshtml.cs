using Defra.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class ContactEmailModel : BasePageModel<ContactEmailModel>
{
    #region UI Models
    [StringLengthMaximum(100, ErrorMessage = "The email address cannot be longer than 100 characters")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDto? Location { get; set; } = new LogisticsLocationDto();
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion
       
    public ContactEmailModel(
        ILogger<ContactEmailModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        OrgId = id;
        EstablishmentId = locationId;
        this.NI_GBFlag = NI_GBFlag;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (!GetType().FullName!.Contains("SelfServe") && _traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        if (NI_GBFlag == "NI")
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
            return await OnGetAsync(OrgId, EstablishmentId, NI_GBFlag ?? string.Empty);
        }

        await SaveEmailToApi();

        if (GetType().FullName!.Contains("SelfServe"))
            return RedirectToPage(
                Routes.Pages.Path.SelfServeConfirmEstablishmentDetailsPath,
                new { id = OrgId, locationId = EstablishmentId, NI_GBFlag });
        else
            return RedirectToPage(
                Routes.Pages.Path.AdditionalEstablishmentAddressPath, 
                new { id = OrgId, NI_GBFlag});
    }

    public IActionResult OnGetChangeEstablishmentAddress(Guid orgId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        if (GetType().FullName!.Contains("SelfServe"))
            return RedirectToPage(
                Routes.Pages.Path.SelfServeEstablishmentNameAndAddressPath,
                new { id = orgId, establishmentId, NI_GBFlag });
        else
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath,
                new { id = orgId, establishmentId, NI_GBFlag });
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
    public virtual bool IsInputValid()
    {
        if (!ModelState.IsValid)
            return false;

        return true;
    }
}
