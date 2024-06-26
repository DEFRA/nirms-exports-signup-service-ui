using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class EligibilityRegulationsModel : BasePageModel<EligibilityRegulationsModel>
{
    [BindProperty]
    public bool Confirmed { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    public EligibilityRegulationsModel(
        ILogger<EligibilityRegulationsModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(EligibilityRegulationsModel), nameof(OnGetAsync));

        OrgId = id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        if (tradeParty != null)
        {
            Confirmed = tradeParty.RegulationsConfirmed;
        }

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }


        _logger.LogInformation("Eligibility Regulations OnGet");

        TradePartyId = tradeParty!.Id;
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(EligibilityRegulationsModel), nameof(OnPostSubmitAsync));

        if (!Confirmed)
        {            
            ModelState.AddModelError(nameof(Confirmed), "Confirm that you have understood the guidance and regulations");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Eligibility Regulations Model validation failed");
            return await OnGetAsync(OrgId);
        }

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        if (tradeParty != null) 
        { 
            tradeParty.RegulationsConfirmed = Confirmed;
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }                

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessCountryPath, 
            new { id = OrgId });
    }
}
