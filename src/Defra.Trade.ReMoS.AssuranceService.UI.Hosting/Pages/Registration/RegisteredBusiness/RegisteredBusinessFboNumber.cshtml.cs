using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessFboNumberModel : PageModel
{
    #region props and ctor

    [BindProperty]
    [Required(ErrorMessage = "Select an option")]
    public string OptionSelected { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-]*$", ErrorMessage = "Enter FBO number using only letters, numbers and hyphens -")]
    public string? FboNumber { get; set; } = string.Empty;

    [BindProperty]
    public Guid TraderId { get; set; }

    private readonly ILogger<RegisteredBusinessFboNumberModel> _logger;
    private readonly ITraderService _traderService;
    public RegisteredBusinessFboNumberModel(ILogger<RegisteredBusinessFboNumberModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    #endregion props and ctor

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("FBO Number OnGet");
        TraderId = Id;

        await PopulateModelProperties();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (OptionSelected == "yes" && string.IsNullOrEmpty(FboNumber))
            ModelState.AddModelError("FboNumber", "Enter FBO number");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        if (OptionSelected == "yes")
        {
            await SaveFboNumberToApiAsync();
            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TraderId });
        }
        else
        {
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessCanNotRegisterPath,
                new { id = TraderId });
        }

    }

    private async Task PopulateModelProperties()
    {
        if (TraderId == Guid.Empty)
            //ERROR
            return;

        FboNumber = await GetFboNumberFromApiAsync();

        if (!string.IsNullOrEmpty(FboNumber))
            OptionSelected = "yes";
        
    }

    private async Task<string?> GetFboNumberFromApiAsync()
    {
        var tradePartyDto = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradePartyDto != null)
        {
            return tradePartyDto.FboNumber;
        }
        return string.Empty;
    }   

    private async Task SaveFboNumberToApiAsync()
    {
        if (TraderId == Guid.Empty)
        {
            //ERROR
        }

        var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);

        if (tradeParty != null)
        {
            tradeParty.FboNumber = FboNumber;
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }
    }
}
