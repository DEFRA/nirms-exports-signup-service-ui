using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;

public class RegisteredBusinessContactPhoneModel : BasePageModel<RegisteredBusinessContactPhoneModel>
{
    
    #region ui model
    [BindProperty]
    // This regex pattern supports various formats of UK phone numbers, including landlines and mobile numbers. It allows for optional spaces in different positions.
    [RegularExpression(
        @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?(\d{4}|\d{3}))|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$", 
        ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]
    [Required(ErrorMessage = "Enter a telephone number")]
    public string PhoneNumber { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public bool? IsAuthorisedSignatory { get; set; }
    [BindProperty]
    public string? PracticeName { get; set; }
    [BindProperty]
    public string? ContactName { get; set; }
    #endregion

    public RegisteredBusinessContactPhoneModel(
        ILogger<RegisteredBusinessContactPhoneModel> logger, 
        ITraderService traderService) : base(logger, traderService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPhoneModel), nameof(OnGetAsync));

        OrgId = id;
        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("PhoneNumber OnGet");
        await GetTradePartyFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPhoneModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitPhone();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryDetailsPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactPhoneModel), nameof(OnPostSaveAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitPhone();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    #region private methods
    private async Task SubmitPhone()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto tradeParty = await GenerateDTOAsync();
        TradePartyId = await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }

    private async Task GetTradePartyFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            PhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
            ContactName = tradeParty.Contact.PersonName ?? string.Empty;
            PracticeName = tradeParty.PracticeName ?? string.Empty;
        }
    }

    private async Task GetIsAuthorisedSignatoryFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            IsAuthorisedSignatory = tradeParty.Contact.IsAuthorisedSignatory;
        }
    }

    private async Task<TradePartyDto> GenerateDTOAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        return new TradePartyDto()
        {
            Id = TradePartyId,
            ApprovalStatus = tradeParty!.ApprovalStatus,
            Contact = new TradeContactDto()
            {
                TelephoneNumber = PhoneNumber,
                IsAuthorisedSignatory = IsAuthorisedSignatory,
                LastModifiedDate = DateTime.UtcNow
            }
        };
    }
    #endregion
}
