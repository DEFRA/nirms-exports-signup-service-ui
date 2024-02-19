using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
[BindProperties]
public class EligibilityRegulationsModel : BasePageModel<EligibilityRegulationsModel>
{
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string? NI_GBFlag { get; set; } = string.Empty;
    public LogisticsLocationDto? Location { get; set; } = new LogisticsLocationDto();
    public string? ButtonText { get; set; } = string.Empty;
    

    public EligibilityRegulationsModel(
        ILogger<EligibilityRegulationsModel> logger, 
        ITraderService traderService,
        IEstablishmentService establishmentService) : base(logger, traderService, establishmentService)
    {}

    public IActionResult OnGetAsync(Guid id, Guid locationId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        OrgId = id;
        EstablishmentId = locationId;
        this.NI_GBFlag = NI_GBFlag;

        if (NI_GBFlag == "NI")
        {
            ButtonText = "Add place of destination";
        }
        else
        {
            ButtonText = "Add place of dispatch";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment eligibility regulations OnPostSubmit");
        await UpdateEstablishmentStatus();
        return RedirectToPage(Routes.Pages.Path.SelfServeEstablishmentAddedPath);
    }

    private async Task UpdateEstablishmentStatus()
    {
        Location = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);

        if (Location != null)
        {
            Location.ApprovalStatus = LogisticsLocationApprovalStatus.Approved;
            await _establishmentService.UpdateEstablishmentDetailsSelfServeAsync(Location);
        }
    }

}
