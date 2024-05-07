using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class AuthorisedSignatoryPositionModel : BasePageModel<AuthorisedSignatoryPositionModel>
{
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
    [StringLengthMaximum(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a position")]
    public string Position { get; set; } = string.Empty;
    [BindProperty]
    public string? BusinessName { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid SignatoryId { get; set; }
    public string? Name { get; set; } = string.Empty;

    public AuthorisedSignatoryPositionModel(
        ITraderService traderService, 
        ILogger<AuthorisedSignatoryPositionModel> logger) : base(traderService, logger)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AuthorisedSignatoryPositionModel), nameof(OnGetAsync));

        OrgId = id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("Position OnGet");

        _ = await GetSignatoryPosFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AuthorisedSignatoryPositionModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryEmailPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AuthorisedSignatoryPositionModel), nameof(OnPostSaveAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
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
            Name = tradeParty.AuthorisedSignatory.Name;
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
            ApprovalStatus = tradeParty!.ApprovalStatus,
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = SignatoryId,
                Name = tradeParty.AuthorisedSignatory?.Name,
                Position = Position,
                EmailAddress = tradeParty.AuthorisedSignatory?.EmailAddress,
				LastModifiedDate = DateTime.UtcNow
			}
        };
    }
    #endregion
}
