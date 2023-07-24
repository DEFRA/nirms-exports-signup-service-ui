using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessBusinessPickerModel : PageModel
{
    #region model properties
    public Dictionary<Guid, string> Businesses { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Select a business")]
    public string SelectedBusiness { get; set; } = default!;
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessBusinessPickerModel> _logger;
    private readonly ITraderService _traderService;

    public RegisteredBusinessBusinessPickerModel(
        ILogger<RegisteredBusinessBusinessPickerModel> logger,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("Business picker OnGet");

        Businesses = await GetDefraBusinessesForUserAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Business picker OnPostSubmit");

        if (string.Equals(SelectedBusiness, "Another business", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("UnregisteredBusiness", "UnregisteredBusiness");
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        /* get business sign-up status from trader service
         * if NEW i.e. no details saved in api, save to api and redirect to country page
         * if INPROGRESS-ELIGIBIMITY, redirect to the next page on eligibility to move to
         * if INPROGRESS, redirect to task list
         * if COMPLETE (T&C checked), redirect to error page
         */

        var partyWithSignUpStatus = await _traderService.GetDefraOrgBusinessSignupStatus(Guid.Parse(SelectedBusiness));
        TraderId = (partyWithSignUpStatus.tradeParty != null) ? partyWithSignUpStatus.tradeParty.Id : Guid.Empty;

        switch (partyWithSignUpStatus.signupStatus)
        {
            case Core.Enums.TradePartySignupStatus.New:
                await SaveSelectedBusinessToApi();
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = TraderId });
            case Core.Enums.TradePartySignupStatus.InProgressEligibilityCountry:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = TraderId });
            case Core.Enums.TradePartySignupStatus.InProgressEligibilityFboNumber:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessFboNumberPath,
                    new { id = TraderId });
            case Core.Enums.TradePartySignupStatus.InProgress:
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = TraderId });
            case Core.Enums.TradePartySignupStatus.Complete:
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                    new { id = TraderId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = TraderId });

    }

    private async Task SaveSelectedBusinessToApi()
    {
        if (TraderId != Guid.Empty)
            return;

        Businesses = await GetDefraBusinessesForUserAsync();

        var partyDto = new TradePartyDTO
        {
            OrgId = Guid.Parse(SelectedBusiness),
            PracticeName = Businesses[Guid.Parse(SelectedBusiness)],
        };

        TraderId = await _traderService.CreateTradePartyAsync(partyDto);
    }

    private static async Task<Dictionary<Guid, string>> GetDefraBusinessesForUserAsync()
    {
        Dictionary<Guid, string> orgsFromAuthToken = new Dictionary<Guid, string>()
        {
            {Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),"ACME Ltd"},
            {Guid.Parse("db070bed-5e34-435d-af82-ab84782e612e"),"ACME2 Ltd"},
            {Guid.Parse("304fe7ec-a56c-4e4f-af7e-e0d48db4eb41"),"ACME3"},
            {Guid.Parse("f9d9d1b5-3c1d-4eb9-92dd-a1419af1b0b3"),"ACME4"},
            {Guid.Parse("26f647b2-7332-4407-aff6-488d8a6ef035"),"ACME5"},
        };

        return await Task.FromResult(orgsFromAuthToken);
    }
}
