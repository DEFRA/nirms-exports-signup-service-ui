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
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_./()&]*$", ErrorMessage = "Name must only include letters, numbers, and special characters -_./()&")]
    [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a name.")]
    public string Name { get; set; } = string.Empty;
    
    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessContactNameModel> _logger;
    private readonly ITraderService _traderService;

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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync(Guid? id = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        TraderId = (TraderId != Guid.Empty) ? TraderId : id ?? Guid.Empty;
        _logger.LogInformation("Name OnGet");

        if (TraderId != Guid.Empty)
        {
            await GetContactNameFromApiAsync();
        }

        return Page();

    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Contact Name OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        TradePartyDTO tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId) ?? new TradePartyDTO();
        tradeParty.Contact ??= new TradeContactDTO();

        tradeParty.Contact.PersonName = Name;


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

    private async Task GetContactNameFromApiAsync()
    {
        TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Name = tradeParty.Contact.PersonName ?? string.Empty;
        }
    }

}
