using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;

public class SignUpConfirmationModel : BasePageModel<SignUpConfirmationModel>
{
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    public string? Email { get; set; } = string.Empty;
    public string StartNowPage { get; set; } = string.Empty;
    public string NI_GBFlag { get; set; } = string.Empty;

    public SignUpConfirmationModel(
        ITraderService traderService, 
        ICheckAnswersService checkAnswersService, 
        IConfiguration config) : base(traderService, checkAnswersService, config)
    {}

    public async Task<IActionResult> OnGet(Guid id)
    {
        OrgId = id;
        TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;
        StartNowPage = _config.GetValue<string>("ExternalLinks:StartNowPage");

        if (TradePartyId != Guid.Empty)
        {
            if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var trader = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            Email = trader?.Contact?.Email;
            NI_GBFlag = RetrieveGB_NIFLAG(trader?.RemosBusinessSchemeNumber!);

            if (!_checkAnswersService.ReadyForCheckAnswers(trader!))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTermsAndConditionsPath,
                    new { id = OrgId });
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