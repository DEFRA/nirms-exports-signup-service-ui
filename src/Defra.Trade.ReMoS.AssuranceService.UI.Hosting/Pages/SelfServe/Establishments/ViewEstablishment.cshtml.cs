using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments
{
    [FeatureGate(FeatureFlags.SelfServeMvpPlus)]
    public class ViewEstablishmentModel : BasePageModel<ViewEstablishmentModel>
    {
        #region UI Model
        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid OrgId { get; set; }
        [BindProperty]
        public string? BusinessName { get; set; }
        [BindProperty]
        public LogisticsLocationDto? LogisticsLocation { get; set; }
        [BindProperty]
        public string? NI_GBFlag { get; set; }
        [BindProperty]
        public string ContentText { get; set; } = "dispatch";
        #endregion

        public ViewEstablishmentModel(
            ILogger<ViewEstablishmentModel> logger,
            IEstablishmentService establishmentService,
            ITraderService traderService) : base(logger, traderService, establishmentService)
                    { }

        public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(ViewEstablishmentModel), nameof(OnGetAsync));

            OrgId = id;
            this.NI_GBFlag = NI_GBFlag;

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
            LogisticsLocation = await _establishmentService.GetEstablishmentByIdAsync(locationId);
            TradePartyId = tradeParty!.Id;
            BusinessName = tradeParty.PracticeName!;

            if (NI_GBFlag == "NI") ContentText = "destination";

            return Page();
        }
    }
}
