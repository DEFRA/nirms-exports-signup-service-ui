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

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class BusinessListModel : BasePageModel<BusinessListModel>
{
    #region model properties
    public List<Organisation> Businesses { get; set; } = default!;
    
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }

    public List<SelectListItem> BusinessSelectList { get; set; } = new()!;
    #endregion
    
    public BusinessListModel(
       ILogger<BusinessListModel> logger,
       ITraderService traderService,
       IUserService userService) : base(logger, traderService, userService)
    { }

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(BusinessListModel), nameof(OnGetAsync));

        await GetDefraOrgsForUserWithApprovalStatus();           
        return Page();
    }

    public IActionResult OnGetNavigateToBusinessDashboard(Guid orgId)
    {
        return RedirectToPage(
            Routes.Pages.Path.UpdatedTermsAndConditionsPath,
            new { id = orgId });
    }

    public async Task<IActionResult> OnGetNavigateToSignup(Guid orgId)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(BusinessListModel), nameof(OnGetNavigateToSignup));

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
        TradePartyId = (business.tradeParty != null) ? business.tradeParty.Id : Guid.Empty;

        switch (business.signupStatus)
        {
            case TradePartySignupStatus.New:
                await SaveSelectedBusinessToApi(orgId);
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = orgId });
            case TradePartySignupStatus.InProgressEligibilityCountry:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = orgId });
            case TradePartySignupStatus.InProgressEligibilityFboNumber:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessFboNumberPath,
                    new { id = orgId });
            case TradePartySignupStatus.InProgressEligibilityRegulations:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = orgId });
            case TradePartySignupStatus.InProgress:
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = orgId });
            case TradePartySignupStatus.Complete:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                    new { id = orgId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = orgId });
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
        if (TradePartyId != Guid.Empty)
            return;

        await GetDefraOrgsForUserWithApprovalStatus();

        var partyDto = new TradePartyDto
        {
            OrgId = orgId,
            PracticeName = Businesses.First(x => x.OrganisationId == orgId).PracticeName,
            ApprovalStatus = TradePartyApprovalStatus.SignupStarted,
        };

        TradePartyId = await _traderService.CreateTradePartyAsync(partyDto);
    }
}
