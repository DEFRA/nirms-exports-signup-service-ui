using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactPositionModel : BasePageModel<RegisteredBusinessContactPositionModel>
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
    [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a position")]
    public string Position { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public Guid ContactId { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
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
        await GetContactPositionFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Contact Position OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitPosition();
        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactEmailPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Contact Position OnPostSave");

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
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto tradeParty = GenerateDTO();
        await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }
    private async Task GetContactPositionFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Position = tradeParty.Contact.Position ?? string.Empty;
        }
    }

    private async Task GetIsAuthorisedSignatoryFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            IsAuthorisedSignatory = tradeParty.Contact.IsAuthorisedSignatory;
        }
    }

    private TradePartyDto GenerateDTO()
    {
        return new TradePartyDto()
        {
            Id = TradePartyId,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                Position = Position,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };
    }
    #endregion
}
