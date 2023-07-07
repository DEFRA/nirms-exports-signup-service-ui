using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

[Authorize]
public class RegisteredBusinessCountryModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Enter a country")]
    public string Country { get; set; } = string.Empty;

    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessCountryModel> _logger;
    private readonly ITraderService _traderService;

    public RegisteredBusinessCountryModel(ILogger<RegisteredBusinessCountryModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Country OnGet");
        TraderId = Id;

        if (!User.Claims.Any())
        {
            var claims = "";
        }

        if (Id != Guid.Empty)
        {
            Country = await GetCountryFromApiAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        await SaveCountryToApiAsync();

        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessFboNumberPath,
            new { id = TraderId });
            }

    private TradePartyDTO CreateDTO()
    {
        TradePartyDTO DTO = new()
        {
            Address = new TradeAddressDTO()
            {
                TradeCountry = Country
            }
        };

        return DTO;
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

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);

        if (tradeParty != null && tradeParty.Address != null)
        {
            tradeParty.Address.TradeCountry = Country;
            await _traderService.UpdateTradePartyAddressAsync(tradeParty);
        }
    }
}
