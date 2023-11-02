using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

public class RegisteredBusinessFboNumberModel : PageModel
{
    #region props and ctor

    [BindProperty]
    [Required(ErrorMessage = "Select if your business has an FBO or PHR number")]
    public string? OptionSelected { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9-]*$", ErrorMessage = "Enter an FBO number using only letters, numbers or hyphens")]
    [MaxLength(25, ErrorMessage = "FBO number must be 25 characters or less")]
    public string? FboNumber { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-]*$", ErrorMessage = "Enter a PHR number using only letters, numbers, spaces or hyphens")]
    [MaxLength(25, ErrorMessage = "PHR number must be 25 characters or less")]
    public string? PhrNumber { get; set; } = string.Empty;

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

        if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(TraderId).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        await PopulateModelProperties(TraderId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Country OnPostSubmit");

        if (ModelState.IsValid)
        {
            if (OptionSelected == "fbo" && string.IsNullOrEmpty(FboNumber))
                ModelState.AddModelError(nameof(FboNumber), "Enter your business' FBO number");

            if (OptionSelected == "phr" && string.IsNullOrEmpty(PhrNumber))
                ModelState.AddModelError(nameof(PhrNumber), "Enter your business' PHR number");
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TraderId);
        }

        await SaveNumberToApiAsync(TraderId);
        if (OptionSelected == "none")
        {      
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessFboPhrGuidancePath,
                new { id = TraderId });
        }
        else
        {
            return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessRegulationsPath,
                new { id = TraderId });
        }

    }

    private async Task PopulateModelProperties(Guid TraderId)
    {
        if (TraderId == Guid.Empty)
            throw new ArgumentNullException(nameof(TraderId));

        TradePartyDto? tradeParty = await GetNumberFromApiAsync();
        
        if(tradeParty != null)
        {
            FboNumber = tradeParty.FboNumber;
            PhrNumber = tradeParty.PhrNumber;
            OptionSelected = tradeParty.FboPhrOption;
        }      
            }

    private async Task<TradePartyDto?> GetNumberFromApiAsync()
    {
        TradePartyDto? tradePartyDto = await _traderService.GetTradePartyByIdAsync(TraderId);
        return tradePartyDto;

    }   

    private async Task SaveNumberToApiAsync(Guid TraderId)
    {
        if (TraderId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(TraderId));
        }
        
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradeParty != null)
        {
            tradeParty.FboNumber = FboNumber;
            tradeParty.PhrNumber = PhrNumber;
            tradeParty.FboPhrOption = OptionSelected;
            await _traderService.UpdateTradePartyAsync(tradeParty);
        }
    }
}
