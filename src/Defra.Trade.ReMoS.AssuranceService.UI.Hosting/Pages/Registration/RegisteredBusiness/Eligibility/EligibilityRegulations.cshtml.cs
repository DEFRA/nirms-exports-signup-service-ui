using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class EligibilityRegulationsModel : BasePageModel<EligibilityRegulationsModel>
{
    [BindProperty]
    public bool Confirmed { get; set; }
    [BindProperty]
    public Guid TraderId { get; set; }
    
    public EligibilityRegulationsModel(
        ILogger<EligibilityRegulationsModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TraderId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(TraderId).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }


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
