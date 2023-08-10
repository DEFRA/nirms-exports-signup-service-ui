using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactNameModel : PageModel
{
    private readonly ILogger<RegisteredBusinessContactNameModel> _logger;
    private readonly ITraderService _traderService;

    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z\s-.]*$", ErrorMessage = "Enter the full name of the contact person using only letters, full stops (.) & hyphens (-)")]
    [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter the name of your business' contact person")]
    public string Name { get; set; } = string.Empty;

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
    public RegisteredBusinessContactNameModel(
        ILogger<RegisteredBusinessContactNameModel> logger,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;
        _logger.LogInformation("Name OnGet");

        await GetContactNameFromApiAsync();

        return Page();

    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Contact Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitName();

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactPositionPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Contact Name OnPostSave");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitName();

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    #region private methods
    private async Task SubmitName()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto tradeParty = GenerateDTO();
        await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }

    private async Task GetContactNameFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Name = tradeParty.Contact.PersonName ?? string.Empty;
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
                PersonName = Name,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };
    }
    #endregion
}
