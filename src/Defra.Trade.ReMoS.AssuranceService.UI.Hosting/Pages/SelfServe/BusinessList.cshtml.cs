using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class BusinessListModel : BasePageModel<BusinessListModel>
{
    #region model properties
    public List<Organisation> Businesses { get; set; } = default!;
    
    public Guid TraderId { get; set; }
    
    public List<SelectListItem> BusinessSelectList { get; set; } = new()!;
    #endregion
    
    public BusinessListModel(
       ILogger<BusinessListModel> logger,
       ITraderService traderService,
       IUserService userService) : base(logger, traderService, userService)
    { }

    public async Task<IActionResult> OnGetAsync()
    {        
        _logger.LogInformation("Business list OnGet");
        await GetDefraOrgsForUserWithApprovalStatus();           
        return Page();
    }

    public async Task<IActionResult> OnGetNavigateToBusinessDashboard(Guid orgId)
    {
        var business = await _traderService.GetDefraOrgBusinessSignupStatus(orgId);
        TraderId = (business.tradeParty != null) ? business.tradeParty.Id : Guid.Empty;

        return RedirectToPage(
            Routes.Pages.Path.SelfServeDashboardPath,
            new { id = TraderId });
    }

    public async Task<IActionResult> OnGetNavigateToSignup(Guid orgId)
    {
        _logger.LogInformation("Business list OnGetNavigateToSignup");

        var orgDetails = _userService.GetOrgDetailsById(User, orgId);
        if (orgDetails == null)
        {
            ModelState.AddModelError(nameof(Businesses), "Business not found, refresh list of businesses");
            return await OnGetAsync();
        }



        if (!orgDetails!.Enrolled)
        {
            if (orgDetails.UserRole == DefraOrgUserRoles.Admin.ToString())
            {
                return RedirectToPage(Routes.Pages.Path.RegisterBusinessForExporterServicePath);
            }
            else if (orgDetails.UserRole == DefraOrgUserRoles.Standard.ToString())
            {
                return RedirectToPage(Routes.Pages.Path.RegisterBusinessForExporterServiceNonAdminPath);
            }
            else
            {
                ModelState.AddModelError(nameof(Businesses), "User role not found");
                return await OnGetAsync();
            }
        }

        var business = await _traderService.GetDefraOrgBusinessSignupStatus(orgId);
        TraderId = (business.tradeParty != null) ? business.tradeParty.Id : Guid.Empty;

        switch (business.signupStatus)
        {
            case TradePartySignupStatus.New:
                await SaveSelectedBusinessToApi(orgId);
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = TraderId });
            case TradePartySignupStatus.InProgressEligibilityCountry:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = TraderId });
            case TradePartySignupStatus.InProgressEligibilityFboNumber:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessFboNumberPath,
                    new { id = TraderId });
            case TradePartySignupStatus.InProgressEligibilityRegulations:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = TraderId });
            case TradePartySignupStatus.InProgress:
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = TraderId });
            case TradePartySignupStatus.Complete:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                    new { id = TraderId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = TraderId });
    }

    public async Task<IActionResult> OnGetRefreshBusinesses()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Index");
    }

    private async Task GetDefraOrgsForUserWithApprovalStatus()
    {
        Businesses = _userService.GetDefraOrgsForUser(User);

        foreach (var business in Businesses)
        {
            business.ApprovalStatus = await _traderService.GetDefraOrgApprovalStatus(business.OrganisationId);
        }
    }
    private async Task SaveSelectedBusinessToApi(Guid orgId)
    {
        if (TraderId != Guid.Empty)
            return;

        await GetDefraOrgsForUserWithApprovalStatus();

        var partyDto = new TradePartyDto
        {
            OrgId = orgId,
            PracticeName = Businesses.First(x => x.OrganisationId == orgId).PracticeName,
            ApprovalStatus = TradePartyApprovalStatus.SignupStarted,
        };

        TraderId = await _traderService.CreateTradePartyAsync(partyDto);
    }
}
