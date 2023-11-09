using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class SelectedBusinessModel : BasePageModel<SelectedBusinessModel>
{
    

    [BindProperty]
    public Guid TradePartyId { get; set; }
    public string SelectedBusinessName { get; set; } = default!;

    public SelectedBusinessModel(
        ILogger<SelectedBusinessModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("SelectedBusiness OnGet");
        TradePartyId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(id).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        SelectedBusinessName = tradeParty?.PracticeName ?? string.Empty;
        
        return Page();
    }
}
