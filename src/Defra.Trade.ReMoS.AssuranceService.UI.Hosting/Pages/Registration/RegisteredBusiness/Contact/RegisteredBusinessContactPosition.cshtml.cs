using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactPositionModel : BasePageModel<RegisteredBusinessContactPositionModel>
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
    [StringLengthMaximum(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a position")]
    public string Position { get; set; } = string.Empty;

    public string BusinessName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    public Guid ContactId { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
    public Guid AuthorisedSignatoryId { get; set; }
    public TradePartyDto TradePartyDto { get; set; } = new TradePartyDto();
    public TradePartyDto? TradePartyDtoCurrent { get; set; } = new TradePartyDto();
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisteredBusinessContactPositionModel(
        ILogger<RegisteredBusinessContactPositionModel> logger,
        ITraderService traderService) : base(logger, traderService)
    {}
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPositionModel), nameof(OnGetAsync));

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
        await GetContactPositionFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPositionModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactEmailPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPositionModel), nameof(OnPostSaveAsync));

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
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto = await GenerateDTOAsync();
        await _traderService.UpdateTradePartyContactAsync(TradePartyDto);
    }
    private async Task GetContactPositionFromApiAsync()
    {
        TradePartyDtoCurrent = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (TradePartyDtoCurrent != null && TradePartyDtoCurrent.Contact != null)
        {
            Position = TradePartyDtoCurrent.Contact.Position ?? string.Empty;
            BusinessName = TradePartyDtoCurrent.PracticeName ?? string.Empty;
            ContactName = TradePartyDtoCurrent.Contact.PersonName ?? string.Empty;
        }
    }

    private async Task GetIsAuthorisedSignatoryFromApiAsync()
    {
        TradePartyDtoCurrent = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (TradePartyDtoCurrent != null && TradePartyDtoCurrent.Contact != null)
        {
            IsAuthorisedSignatory = TradePartyDtoCurrent.Contact.IsAuthorisedSignatory;
            if (TradePartyDtoCurrent!.AuthorisedSignatory != null)
            {
                AuthorisedSignatoryId = TradePartyDtoCurrent.AuthorisedSignatory.Id;
            }
        }
    }

    private async Task<TradePartyDto> GenerateDTOAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        var tradePartyDto = new TradePartyDto()
        {
            Id = TradePartyId,
            ApprovalStatus = tradeParty!.ApprovalStatus,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                Position = Position,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };

        if (IsAuthorisedSignatory == true)
        {
            tradePartyDto.AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = AuthorisedSignatoryId,
                EmailAddress = TradePartyDtoCurrent!.Contact!.Email,
                Name = TradePartyDtoCurrent.Contact.PersonName,
                Position = Position
            };
        }

        return tradePartyDto;
    }
    #endregion
}
