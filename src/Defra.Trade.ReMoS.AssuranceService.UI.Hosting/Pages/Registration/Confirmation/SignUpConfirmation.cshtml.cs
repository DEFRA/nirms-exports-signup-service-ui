using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;

public class SignUpConfirmationModel : PageModel
{
    [BindProperty]
    public Guid TraderId { get; set; }

    public string? Email { get; set; } = string.Empty;
    public string StartNowPage { get; set; } = string.Empty;
    public string NI_GBFlag { get; set; } = string.Empty;

    private readonly ITraderService _traderService;
    private readonly ICheckAnswersService _checkAnswersService;
    private readonly IConfiguration _config;

    public SignUpConfirmationModel(ITraderService traderService, ICheckAnswersService checkAnswersService, IConfiguration config)
    {
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        _checkAnswersService = checkAnswersService ?? throw new ArgumentNullException(nameof(checkAnswersService));
        _config = config;
    }

    public async Task<IActionResult> OnGet(Guid id)
    {
        TraderId = id;
        StartNowPage = _config.GetValue<string>("ExternalLinks:StartNowPage");

        if (TraderId != Guid.Empty)
        {
            if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var trader = await _traderService.GetTradePartyByIdAsync(TraderId);
            Email = trader?.Contact?.Email;
            NI_GBFlag = RetrieveGB_NIFLAG(trader?.RemosBusinessSchemeNumber!);

            if (!_checkAnswersService.ReadyForCheckAnswers(trader!))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTermsAndConditionsPath,
                    new { id = TraderId });
            }
        }
        return Page();
    }
    
    public string RetrieveGB_NIFLAG(string remosNumber)
    {
        var nIGbFlagMatch = Regex.Match(remosNumber, @"\b(?:NI|GB)\b", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        return nIGbFlagMatch.Value;
    }
}