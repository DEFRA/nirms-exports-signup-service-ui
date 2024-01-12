using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class SelectedBusinessModel : BasePageModel<SelectedBusinessModel>
{
    

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    public string SelectedBusinessName { get; set; } = default!;

    public SelectedBusinessModel(
        ILogger<SelectedBusinessModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("SelectedBusiness OnGet");
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

        SelectedBusinessName = tradeParty?.PracticeName ?? string.Empty;
        
        return Page();
    }
}
