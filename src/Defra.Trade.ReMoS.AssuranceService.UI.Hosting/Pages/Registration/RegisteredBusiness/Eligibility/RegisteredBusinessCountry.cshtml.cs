using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessCountryModel : PageModel
{
    #region ui model variables
    [BindProperty]
    public string? Country { get; set; } = string.Empty;

    [BindProperty]
    //[Required(ErrorMessage = "Select what your business will do under the scheme")]
    public string? GBChosen { get; set; }

    [BindProperty]
    public Guid TraderId { get; set; }
    [BindProperty]
    public bool CountrySaved { get; set; }

    public bool AllowedToTasklist { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessCountryModel> _logger;
    private readonly ITraderService _traderService;
    private readonly ICheckAnswersService _checkAnswersService;

    public RegisteredBusinessCountryModel(ILogger<RegisteredBusinessCountryModel> logger, ITraderService traderService, ICheckAnswersService checkAnswersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        _checkAnswersService = checkAnswersService ?? throw new ArgumentNullException(nameof(checkAnswersService));
    }

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Country OnGet");
        TraderId = Id;

        if (Id != Guid.Empty)
        {
            if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(TraderId).Result)
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            Country = await GetCountryFromApiAsync();
            CountrySaved = !string.IsNullOrEmpty(Country);
        }

        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(Id);
        if (_checkAnswersService.GetEligibilityProgress(tradeParty!) == TaskListStatus.COMPLETE)
        {
            AllowedToTasklist = true;
        }

        if (Country != "")
        {
            if (Country == "NI")
            {
                GBChosen = "recieve";
            }
            else{
                GBChosen = "send";
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (!CountrySaved)
        {
            CheckVariables();
        }
        

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        if (CountrySaved)
        {
            return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessFboNumberPath,
            new { id = TraderId });
        }

        await SaveCountryToApiAsync();

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessFboNumberPath,
            new { id = TraderId });
    }

    #region private methods
    private TradePartyDto CreateDTO()
    {
        TradePartyDto DTO = new()
        {
            Address = new TradeAddressDto()
            {
                TradeCountry = Country
            }
        };

        return DTO;
    }

    private void CheckVariables()
    {
        if (GBChosen == "" || GBChosen == null)
        {
            ModelState.AddModelError(nameof(GBChosen), "Select what your business will do under the scheme");
            return;
        }

        if (GBChosen == "recieve")
        {
            Country = "NI";
        }

        if (Country == "")
        {
            ModelState.AddModelError(nameof(Country), "Select a location");
        }
    }

    private async Task<string> GetCountryFromApiAsync()
    {
        var tradePartyDto = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradePartyDto != null && tradePartyDto.Address != null && tradePartyDto.Address.TradeCountry != null)
        {
            return tradePartyDto.Address.TradeCountry;
        }
        return string.Empty;
    }

    private async Task SaveCountryToApiAsync()
    {
        if (TraderId == Guid.Empty)
        {
            TraderId = await _traderService.CreateTradePartyAsync(CreateDTO());
            return;
        }

        var tradeAddress = new TradeAddressDto { TradeCountry = Country };
        await _traderService.AddTradePartyAddressAsync(TraderId, tradeAddress);

    }
    #endregion
}
