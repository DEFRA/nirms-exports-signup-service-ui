using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class ConfirmRemoveEstablishmentModel : BasePageModel<ConfirmRemoveEstablishmentModel>
{
    #region UI Model
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid EstablishmentId { get; set; }
    public LogisticsLocationDto? Establishment { get; set; }
    public string DispatchOrDestination { get; set; } = default!;
    [BindProperty]
    public string? NI_GBFlag { get; set; }
    #endregion

    public ConfirmRemoveEstablishmentModel(
      ILogger<ConfirmRemoveEstablishmentModel> logger,
      IEstablishmentService establishmentService,
      ITraderService traderService) : base(logger, traderService, establishmentService)
    { }


    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(ConfirmRemoveEstablishmentModel), nameof(OnGetAsync));

        OrgId = id;
        EstablishmentId = locationId;
        this.NI_GBFlag = NI_GBFlag;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);

        if (tradeParty != null)
        {
            TradePartyId = tradeParty.Id;
        }

        Establishment = await _establishmentService.GetEstablishmentByIdAsync(locationId);

        if (Establishment!.ApprovalStatus != LogisticsLocationApprovalStatus.Approved)
        {
            return RedirectToPage(Routes.Pages.Path.EstablishmentErrorPath, new { id = OrgId });
        }

        if (NI_GBFlag == "NI")
        {
            DispatchOrDestination = "destination";
        }
        else
        {
            DispatchOrDestination = "dispatch";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(ConfirmRemoveEstablishmentModel), nameof(OnPostSubmitAsync));

        Establishment = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);

        if (Establishment is not null)
        {
            Establishment.ApprovalStatus = LogisticsLocationApprovalStatus.Removed;
            Establishment.LastModifiedDate = DateTime.UtcNow;
            await _establishmentService.UpdateEstablishmentDetailsSelfServeAsync(Establishment);
        }

        return RedirectToPage(
                Routes.Pages.Path.SelfServeEstablishmentRemovedPath,
                new { id = OrgId, establishmentName = Establishment?.Name, NI_GBFlag = this.NI_GBFlag });
    }
}
