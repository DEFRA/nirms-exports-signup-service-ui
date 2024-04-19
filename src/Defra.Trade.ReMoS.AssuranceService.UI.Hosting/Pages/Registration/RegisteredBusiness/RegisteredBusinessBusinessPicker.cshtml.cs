using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessBusinessPickerModel : BasePageModel<RegisteredBusinessBusinessPickerModel>
{
    #region model properties
    public List<Organisation> Businesses { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Select a business")]
    public string? SelectedBusiness { get; set; } = default!;
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public List<SelectListItem> BusinessSelectList { get; set; } = new()!;
    #endregion

    
    public RegisteredBusinessBusinessPickerModel(
        ILogger<RegisteredBusinessBusinessPickerModel> logger,
        ITraderService traderService,
        IUserService userService) : base(logger, traderService, userService)
    {}

    public IActionResult OnGet()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessBusinessPickerModel), nameof(OnGet));

        Businesses = _userService.GetDefraOrgsForUser(User);

        if (Businesses?.Count > 7)
        {
            BuildBusinessSelectList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessBusinessPickerModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return OnGet();
        }

        if (string.Equals(SelectedBusiness, "Choose business", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            SelectedBusiness = null;
            ModelState.AddModelError("SelectedBusiness", "Select a business");
            return OnGet();
        }

        if (string.Equals(SelectedBusiness, "Another business", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessPickerNoBusinessPickedPath);
        }

        if (!Guid.TryParse(SelectedBusiness, out _))
        {
            ModelState.AddModelError(nameof(SelectedBusiness), "Guid for Selected Business is not valid");
            return OnGet();
        }

        var orgDetails = _userService.GetOrgDetailsById(User, Guid.Parse(SelectedBusiness!));
        if (orgDetails == null)
        {
            ModelState.AddModelError(nameof(SelectedBusiness), "Business not found, refresh list of businesses");
            return OnGet();
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
                ModelState.AddModelError(nameof(SelectedBusiness), "User role not found");
                return OnGet();
            }
        }

        /* get business sign-up status from trader service
         * if NEW i.e. no details saved in api, save to api and redirect to country page
         * if INPROGRESS-ELIGIBIMITY, redirect to the next page on eligibility to move to
         * if INPROGRESS, redirect to task list
         * if COMPLETE (T&C checked), redirect to error page
         */

        OrgId = Guid.Parse(SelectedBusiness);
        var (tradeParty, signupStatus) = await _traderService.GetDefraOrgBusinessSignupStatus(OrgId);
        TradePartyId = (tradeParty != null) ? tradeParty.Id : Guid.Empty;

        switch (signupStatus)
        {
            case TradePartySignupStatus.New:
                await SaveSelectedBusinessToApi();
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = OrgId });
            case TradePartySignupStatus.InProgressEligibilityCountry:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = OrgId });
            case TradePartySignupStatus.InProgressEligibilityFboNumber:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessFboNumberPath,
                    new { id = OrgId });
            case TradePartySignupStatus.InProgressEligibilityRegulations:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                    new { id = OrgId });
            case TradePartySignupStatus.InProgress:
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = OrgId });
            case TradePartySignupStatus.Complete:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                    new { id = OrgId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = OrgId });
    }

    private void BuildBusinessSelectList()
    {
        BusinessSelectList.AddRange(Businesses.Select(keyValuePair => new SelectListItem()
        {
            Value = keyValuePair.OrganisationId.ToString(),
            Text = keyValuePair.PracticeName
        }));

        BusinessSelectList.Insert(0, new SelectListItem("Choose business", null));
        BusinessSelectList.Insert(BusinessSelectList.Count, new SelectListItem("Another business", null));
    }


    private async Task SaveSelectedBusinessToApi()
    {
        if (TradePartyId != Guid.Empty)
            return;

        Businesses = _userService.GetDefraOrgsForUser(User);

        var partyDto = new TradePartyDto
        {
            OrgId = Guid.Parse(SelectedBusiness!),
            PracticeName = Businesses.First(x => x.OrganisationId == Guid.Parse(SelectedBusiness!)).PracticeName,
        };

        TradePartyId = await _traderService.CreateTradePartyAsync(partyDto);
    }

    public async Task<IActionResult> OnGetRefreshBusinesses()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Index");
    }
}
