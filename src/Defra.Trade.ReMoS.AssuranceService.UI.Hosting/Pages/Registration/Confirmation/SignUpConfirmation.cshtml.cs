using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;

public class SignUpConfirmationModel : PageModel
{
    [BindProperty]
    public Guid TraderId { get; set; }
    public string? Email { get; set; } = string.Empty;

    private readonly ITraderService _traderService;

    public SignUpConfirmationModel(ITraderService traderService)
    {
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));

    }
    public async Task<IActionResult> OnGet(Guid id)
    {
        TraderId = id;

        if (TraderId != Guid.Empty)
        {
            var trader = await _traderService.GetTradePartyByIdAsync(TraderId);
            Email = trader?.Contact?.Email;
        }

        return Page();
    }
}
