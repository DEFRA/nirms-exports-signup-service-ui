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
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [StringLength(100, ErrorMessage = "Email is too long")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDTO? Location { get; set; } = new LogisticsLocationDTO();
    public LogisticsLocationBusinessRelationshipDTO? LogisticsLocationBusinessRelationship { get; set; } = new LogisticsLocationBusinessRelationshipDTO();
    public Guid TradePartyId { get; set; }
    public Guid EstablishmentId { get; set; }
    #endregion

    private readonly IEstablishmentService _establishmentService;
    private readonly ITraderService _traderService;
    private readonly ILogger<ContactEmailModel> _logger;

    public ContactEmailModel(
        ILogger<ContactEmailModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId)
    {
        _logger.LogInformation("Establishment departure destination OnGetAsync");
        TradePartyId = id;
        EstablishmentId = locationId;

        if (TradePartyId != Guid.Empty)
        {
            Location = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);
            LogisticsLocationBusinessRelationship = await _establishmentService
                .GetRelationshipBetweenPartyAndEstablishment(
                TradePartyId,
                EstablishmentId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment contact email OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, EstablishmentId);
        }

        await SaveEmailToApi();

        return RedirectToPage(Routes.Pages.Path.AdditionalEstablishmentDepartureAddressPath, new { id = TradePartyId });
    }

    private async Task SaveEmailToApi()
    {
        LogisticsLocationBusinessRelationship = await _establishmentService.GetRelationshipBetweenPartyAndEstablishment(TradePartyId, EstablishmentId);

        if (LogisticsLocationBusinessRelationship != null)
        {
            LogisticsLocationBusinessRelationship.ContactEmail = Email;
            await _establishmentService.UpdateEstablishmentRelationship(LogisticsLocationBusinessRelationship);
        }
    }
}
