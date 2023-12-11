using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
[ExcludeFromCodeCoverage]
public class BusinessListModel : BasePageModel<BusinessListModel>
{
    #region model properties
    public List<Organisation> Businesses { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Select a business")]
    public string? SelectedBusiness { get; set; } = default!;
    public Guid TraderId { get; set; }
    public List<SelectListItem> BusinessSelectList { get; set; } = new()!;
    #endregion
    public BusinessListModel(
       ILogger<BusinessListModel> logger,
       ITraderService traderService,
       IUserService userService) : base(logger, traderService, userService)
    { }

    public IActionResult OnGet()
    {
        _logger.LogInformation("Business picker OnGet");
        Businesses = _userService.GetDefraOrgsForUser(User);

        if (Businesses?.Count > 7)
        {
            BuildBusinessSelectList();
        }

        return Page();
    }

    private void BuildBusinessSelectList()
    {
        BusinessSelectList.AddRange(Businesses.Select(keyValuePair => new SelectListItem()
        {
            Value = keyValuePair.OrganisationId.ToString(),
            Text = keyValuePair.PracticeName
        }));
    }

    public async Task<IActionResult> OnGetNavigateToBusinessDashboard(Guid orgId)
    {
        var business = await _traderService.GetDefraOrgBusinessSignupStatus(orgId);
        TraderId = (business.tradeParty != null) ? business.tradeParty.Id : Guid.Empty;

        return RedirectToPage(
            Routes.Pages.Path.SelfServeDashboardPath,
            new { id = TraderId });
    }
}
