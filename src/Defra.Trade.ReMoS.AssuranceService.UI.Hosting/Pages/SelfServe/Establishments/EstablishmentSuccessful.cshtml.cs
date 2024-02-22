using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments
{
    [FeatureGate(FeatureFlags.SelfServeMvpPlus)]
    public class EstablishmentSuccessfulModel : BasePageModel<EstablishmentSuccessfulModel>
    {
        #region UI Model
        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid OrgId { get; set; }
        [BindProperty]
        public Guid EstablishmentId { get; set; }

        public LogisticsLocationDto? Establishment { get; set; }
        public string Heading { get; set; } = default!;
        public string DispatchOrDestination { get; set; } = default!;
        #endregion

        public EstablishmentSuccessfulModel(
        ILogger<EstablishmentSuccessfulModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
        { }

        public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
        {
            _logger.LogInformation("Establishment dispatch destination OnGetAsync");
            OrgId = id;
            EstablishmentId = locationId;

            var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);

            if (tradeParty != null)
            {
                TradePartyId = tradeParty.Id;
            }

            Establishment = await _establishmentService.GetEstablishmentByIdAsync(locationId);

            if (NI_GBFlag == "NI")
            {
                Heading = "Place of destination successfully added";
                DispatchOrDestination = "destination";
            }
            else
            {
                Heading = "Place of dispatch successfully added";
                DispatchOrDestination = "dispatch";
            }

            return Page();
        }

        public IActionResult OnPostSubmit()
        {
            return RedirectToPage(
                    Routes.Pages.Path.SelfServeDashboardPath,
                    new { id = OrgId });
        }
    }
}
