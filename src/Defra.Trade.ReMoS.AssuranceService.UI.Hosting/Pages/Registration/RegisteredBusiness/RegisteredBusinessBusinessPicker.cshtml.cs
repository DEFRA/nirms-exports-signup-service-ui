using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessBusinessPickerModel : PageModel
{
    #region model properties
    public Dictionary<Guid, string> Businesses { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Select a business")]
    public string SelectedBusiness { get; set; } = default!;
    public Guid TraderId { get; set; }
    public Guid BusinessId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessBusinessPickerModel> _logger;
    private readonly ITraderService _traderService;
    private readonly IUserService _userService;

    public RegisteredBusinessBusinessPickerModel(
        ILogger<RegisteredBusinessBusinessPickerModel> logger,
        ITraderService traderService,
        IUserService userService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("Business picker OnGet");
        Businesses = _userService.GetDefraOrgsForUser(User);
        Businesses = new Dictionary<Guid, string>
        {
            {Guid.NewGuid(), "AdrianLtd1" },
            {Guid.NewGuid(), "AdrianLtd2" },
            {Guid.NewGuid(), "AdrianLtd3" },
            {Guid.NewGuid(), "AdrianLtd4" },
            {Guid.NewGuid(), "AdrianLtd5" },
            {Guid.NewGuid(), "AdrianLtd6" },
            {Guid.NewGuid(), "AdrianLtd7" },
            {Guid.NewGuid(), "AdrianLtd8" }
        };

        var selectList = new SelectList(Businesses)
        
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

        Businesses = _userService.GetDefraOrgsForUser(User);

        var partyDto = new TradePartyDTO
        {
            OrgId = Guid.Parse(SelectedBusiness),
            PracticeName = Businesses[Guid.Parse(SelectedBusiness)],
        };

        TraderId = await _traderService.CreateTradePartyAsync(partyDto);
    }
}
