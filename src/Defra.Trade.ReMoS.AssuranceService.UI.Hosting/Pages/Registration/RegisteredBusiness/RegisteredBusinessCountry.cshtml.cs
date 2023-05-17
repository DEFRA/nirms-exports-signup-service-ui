using Defra.Trade.ReMoS.AssuranceService.API.Domain.DTO;
using Defra.Trade.ReMoS.AssuranceService.API.Domain.Entities;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
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
    #endregion

    private readonly ILogger<RegisteredBusinessCountryModel> _logger;
    private ITraderService _traderService;

    /// <summary>
    /// Constructor. 
    /// </summary>
    /// <param name="logger">Application logging.</param>
    /// <exception cref="ArgumentNullException">Guard statement reaction.</exception>
    public RegisteredBusinessCountryModel(ILogger<RegisteredBusinessCountryModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    //Remove warning when API integration added (has to be async for OnPost functionality but throws this error)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Country OnGet");
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        TradePartyDTO traderDTO = CreateDTO();
        await _traderService.CreateTradePartyAsync(traderDTO);

        return Redirect(Routes.RegistrationTasklist);
    }

    private TradePartyDTO CreateDTO()
    {
        TradePartyDTO DTO = new()
        {
            CountryName = Country
        };
     return DTO;
    }
}
