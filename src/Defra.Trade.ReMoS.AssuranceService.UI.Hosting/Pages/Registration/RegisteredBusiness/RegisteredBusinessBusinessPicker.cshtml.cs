using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public Guid TraderId { get; set; }
    public List<SelectListItem> BusinessSelectList { get; set; } = new()!;
    #endregion

    
    public RegisteredBusinessBusinessPickerModel(
        ILogger<RegisteredBusinessBusinessPickerModel> logger,
        ITraderService traderService,
        IUserService userService) : base(logger, traderService, userService)
    {}

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

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Business picker OnPostSubmit");

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

        var (tradeParty, signupStatus) = await _traderService.GetDefraOrgBusinessSignupStatus(Guid.Parse(SelectedBusiness));
        TraderId = (tradeParty != null) ? tradeParty.Id : Guid.Empty;

        switch (signupStatus)
        {
            case TradePartySignupStatus.New:
                await SaveSelectedBusinessToApi();
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
        if (TraderId != Guid.Empty)
            return;

        Businesses = _userService.GetDefraOrgsForUser(User);

        var partyDto = new TradePartyDto
        {
            OrgId = Guid.Parse(SelectedBusiness!),
            PracticeName = Businesses.First(x => x.OrganisationId == Guid.Parse(SelectedBusiness!)).PracticeName,
        };

        TraderId = await _traderService.CreateTradePartyAsync(partyDto);
    }

    public async Task<IActionResult> OnGetRefreshBusinesses()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToPage("/Index");
    }
}
