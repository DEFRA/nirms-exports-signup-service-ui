using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessCountryModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Enter a country")]
    public string Country { get; set; } = string.Empty;

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

        await GetCountry();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        TraderDTO traderDTO = CreateDTO();
        Guid tp = await _traderService.CreateTradePartyAsync(traderDTO);

        return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new {id = tp });
    }

    private TraderDTO CreateDTO()
    {
        TraderDTO DTO = new()
        {
            Address = new TradeAddress()
            {
                TradeCountry = Country
            }
        };
     return DTO;
    }

    private async Task<string?> GetCountry()
    {
        TradeParty? tp = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tp != null && tp.TradeAddress != null)
        {
            return tp.TradeAddress.TradeCountry;
        }
        return "";
    }
}
