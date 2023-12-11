using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class SelfServeDashboardModel : BasePageModel<SelfServeDashboardModel>
{
    #region ui model variables
    [BindProperty]
    public Guid RegistrationID { get; set; }
    public string BusinessName { get; set; } = default!;
    public string RmsNumber { get; set; } = default!;
    #endregion

    public SelfServeDashboardModel(
           ILogger<SelfServeDashboardModel> logger,
           ITraderService traderService,
           IEstablishmentService establishmentService,
           ICheckAnswersService checkAnswersService)
           : base(logger, traderService, establishmentService, checkAnswersService)
    { }
    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("OnGet Self serve dashboard");

        RegistrationID = Id;

        if (!_traderService.ValidateOrgId(User.Claims, RegistrationID).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        await GetAPIData();

        return Page();
    }

    private async Task GetAPIData()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

        if (tradeParty != null && tradeParty.Id != Guid.Empty)
        {
            BusinessName = tradeParty.PracticeName ?? string.Empty;
            RmsNumber = tradeParty.RemosBusinessSchemeNumber ?? string.Empty;
        }
    }
}
