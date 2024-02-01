using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[ExcludeFromCodeCoverage]
[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class ConfirmEstablishmentDetailsModel : BasePageModel<ConfirmEstablishmentDetailsModel>
{
    #region UI Models
    [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string? Email { get; set; } = string.Empty;
    public LogisticsLocationDto? Location { get; set; } = new LogisticsLocationDto();
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public Guid EstablishmentId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string Country { get; set; } = default!;
    public string? BusinessName { get; set; }
    #endregion

    public ConfirmEstablishmentDetailsModel(
        ILogger<ConfirmEstablishmentDetailsModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid locationId, string country)
    {
        _logger.LogInformation("Establishment dispatch destination OnGetAsync");
        OrgId = id;
        EstablishmentId = locationId;
        Country = country;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);

        if (tradeParty != null)
        {
            TradePartyId = tradeParty.Id;
            BusinessName = tradeParty.PracticeName;
        }

        return Page();
    }
}
