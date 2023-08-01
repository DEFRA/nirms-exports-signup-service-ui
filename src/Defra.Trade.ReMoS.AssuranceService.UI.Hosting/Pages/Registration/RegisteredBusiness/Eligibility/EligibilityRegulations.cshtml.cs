using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class EligibilityRegulationsModel : PageModel
{
    [BindProperty]
    public bool Confirmed { get; set; }
    [BindProperty]
    public Guid TraderId { get; set; }
    private readonly ILogger<EligibilityRegulationsModel> _logger;
    private readonly ITraderService _traderService;

    public EligibilityRegulationsModel(ILogger<EligibilityRegulationsModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TraderId = id;
        _logger.LogInformation("Eligibility Regulations OnGet");

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);

        if (tradeParty != null)
        {
            Confirmed = tradeParty.RegulationsConfirmed;
        }
            return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Eligibility Regulations On Post Submit");
        
        if (!Confirmed)
        {            
            ModelState.AddModelError(nameof(Confirmed), "Confirm that you have understood the guidance and regulations");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Eligibility Regulations Model validation failed");
            return await OnGetAsync(TraderId);
        }

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);

        if (tradeParty != null) 
        { 
            tradeParty.RegulationsConfirmed = Confirmed;
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }                

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath, 
            new { id = TraderId });
    }
}
