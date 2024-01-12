using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;
public class RegisteredBusinessCountryModel : BasePageModel<RegisteredBusinessCountryModel>
{
    #region ui model variables
    [BindProperty]
    public string? Country { get; set; } = string.Empty;

    [BindProperty]
    //[Required(ErrorMessage = "Select what your business will do under the scheme")]
    public string? GBChosen { get; set; }

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public bool CountrySaved { get; set; }

    [BindProperty]
    public string? PracticeName { get; set; } = string.Empty;
    #endregion

    public RegisteredBusinessCountryModel(
        ILogger<RegisteredBusinessCountryModel> logger,
        ITraderService traderService,
        ICheckAnswersService checkAnswersService) : base(logger, traderService, checkAnswersService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Country OnGet");
        OrgId = Id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (Id != Guid.Empty)
        {
            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(tradeParty))
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            Country = await GetCountryFromApiAsync();
            CountrySaved = !string.IsNullOrEmpty(Country);

            if (CountrySaved)
            {
                return RedirectToPage(Routes.Pages.Path.RegisteredBusinessCountryStaticPath,
                new { id = OrgId });
            }
        }

        PracticeName = tradeParty?.PracticeName ?? string.Empty;

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
            return await OnGetAsync(OrgId);
        }

        if (CountrySaved)
        {
            return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
        }

        await SaveCountryToApiAsync();

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
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
            ModelState.AddModelError(nameof(GBChosen), $"Select what {PracticeName} will do under the scheme");
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
        var tradePartyDto = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradePartyDto != null && tradePartyDto.Address != null && tradePartyDto.Address.TradeCountry != null)
        {
            return tradePartyDto.Address.TradeCountry;
        }
        return string.Empty;
    }

    private async Task SaveCountryToApiAsync()
    {
        if (TradePartyId == Guid.Empty)
        {
            TradePartyId = await _traderService.CreateTradePartyAsync(CreateDTO());
            return;
        }

        var tradeAddress = new TradeAddressDto { TradeCountry = Country };
        await _traderService.AddTradePartyAddressAsync(TradePartyId, tradeAddress);

    }
    #endregion
}
