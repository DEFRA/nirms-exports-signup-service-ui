using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class AuthorisedSignatoryPositionModel : PageModel
{
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
    [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a position")]
    public string Position { get; set; } = string.Empty;
    [BindProperty]
    public string? BusinessName { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid SignatoryId { get; set; }

    private readonly ITraderService _traderService;
    private readonly ILogger<AuthorisedSignatoryPositionModel> _logger;

    public AuthorisedSignatoryPositionModel(ITraderService traderService, ILogger<AuthorisedSignatoryPositionModel> logger)
    {
        _traderService = traderService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(id).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("Position OnGet");

        _ = await GetSignatoryPosFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Signatory Position OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryEmailPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Signatory Position OnPostSave");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    #region private methods
    private async Task SubmitPosition()
    {
        var tradeParty = await GenerateDTO();
        await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);
    }

    private async Task<TradePartyDto?> GetSignatoryPosFromApiAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
        {
            SignatoryId = tradeParty.AuthorisedSignatory.Id;
            Position = string.IsNullOrEmpty(Position) ? tradeParty.AuthorisedSignatory.Position ?? "" : Position;
            BusinessName = tradeParty.PracticeName;

            return tradeParty;
        }

        return null;
    }

    private async Task<TradePartyDto> GenerateDTO()
    {
        var tradeParty = await GetSignatoryPosFromApiAsync();
        return new TradePartyDto()
        {
            Id = TradePartyId,
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = SignatoryId,
                Name = tradeParty?.AuthorisedSignatory?.Name,
                Position = Position,
                EmailAddress = tradeParty?.AuthorisedSignatory?.EmailAddress
            }
        };
    }
    #endregion
}
