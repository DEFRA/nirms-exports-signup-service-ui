using Defra.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class ContactEmailModel : PageModel
{
    #region UI Models
    [RegularExpression(
       @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
       ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [StringLength(100, ErrorMessage = "Email is too long")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDTO? Location { get; set; } = new LogisticsLocationDTO();
    public Guid TradePartyId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion

    private readonly IEstablishmentService _establishmentService;
    private readonly ILogger<ContactEmailModel> _logger;

    public ContactEmailModel(
        ILogger<ContactEmailModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        TradePartyId = id;
        EstablishmentId = locationId;
        this.NI_GBFlag = NI_GBFlag;

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a place of destination (optional)";
            ContentText = "Add all establishments in Northern Ireland where your goods go after the port of entry. For example, a hub or store.";
        }
        else
        {
            ContentHeading = "Add a place of dispatch";
            ContentText = "Add all establishments in Great Britan from which your goods will be departing under the scheme.";
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

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, EstablishmentId, NI_GBFlag ?? string.Empty);
        }

        await SaveEmailToApi();

        return RedirectToPage(
            Routes.Pages.Path.AdditionalEstablishmentAddressPath, 
            new { id = TradePartyId, NI_GBFlag});
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

    public IActionResult OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        return RedirectToPage(
            Routes.Pages.Path.EstablishmentNameAndAddressPath,
            new { id = tradePartyId, establishmentId, NI_GBFlag });
    }
}
