using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessContactPositionModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a position using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter the position of the contact person")]
    public string Position { get; set; } = string.Empty;

    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessContactPositionModel> _logger;
    private readonly ITraderService _traderService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RegisteredBusinessContactPositionModel(
        ILogger<RegisteredBusinessContactPositionModel> logger,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService;
    }

    public async Task<IActionResult> OnGetAsync(Guid? id = null)
    {
        TraderId = (TraderId != Guid.Empty) ? TraderId : id ?? Guid.Empty;
        _logger.LogInformation("Position OnGet");

        if (TraderId != Guid.Empty)
        {
            await GetContactPositionFromApiAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Contact Position OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        TradePartyDTO tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId) ?? new TradePartyDTO();
        tradeParty.Contact ??= new TradeContactDTO();

        tradeParty.Contact.Position = Position;


        if (tradeParty.Id == Guid.Empty)
        {
            TraderId = await _traderService.CreateTradePartyAsync(tradeParty);
        }
        else
        {
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TraderId });
    }
    private async Task GetContactPositionFromApiAsync()
    {
        TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Position = tradeParty.Contact.Position ?? string.Empty;
        }
    }
}
