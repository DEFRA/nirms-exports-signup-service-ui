using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class SelectedBusinessModel : PageModel
{
    

    [BindProperty]
    public Guid TradePartyId { get; set; }
    public string SelectedBusinessName { get; set; } = default!;

    private readonly ILogger<SelectedBusinessModel> _logger;
    private readonly ITraderService _traderService;

    public SelectedBusinessModel(ILogger<SelectedBusinessModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("SelectedBusiness OnGet");
        TradePartyId = id;

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        SelectedBusinessName = tradeParty?.PracticeName ?? string.Empty;
        
        return Page();
    }
}
