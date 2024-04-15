using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Eligibility;

public class RegisteredBusinessCountryStaticModel : BasePageModel<RegisteredBusinessCountryStaticModel>
{
    #region ui model variables
    [BindProperty]
    public string? Country { get; set; } = string.Empty;
    [BindProperty]
    public string ContentText { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    #endregion

    public RegisteredBusinessCountryStaticModel (
        ILogger<RegisteredBusinessCountryStaticModel> logger,
        ITraderService traderService) : base(logger, traderService)
        { }

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessCountryStaticModel), nameof(OnGetAsync));

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
            ContentText = Country == "NI" ? "receiving" : "sending";
        }

        return Page();
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
}
