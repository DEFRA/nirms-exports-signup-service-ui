using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;

public class RegisteredBusinessContactPhoneModel : PageModel
{
    private readonly ITraderService _traderService;
    private readonly ILogger<RegisteredBusinessContactPhoneModel> _logger;

    #region ui model
    [BindProperty]
    // This regex pattern supports various formats of UK phone numbers, including landlines and mobile numbers. It allows for optional spaces in different positions.
    [RegularExpression(
        @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?(\d{4}|\d{3}))|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$", 
        ErrorMessage = "Enter a telephone number in the correct format, like 01632 960 001, 07700 900 982 or +44 808 157 019")]
    [Required(ErrorMessage = "Enter the phone number of the contact person")]
    public string PhoneNumber { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public bool? IsAuthorisedSignatory { get; set; }
    #endregion

    public RegisteredBusinessContactPhoneModel(ILogger<RegisteredBusinessContactPhoneModel> logger, ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;
        _logger.LogInformation("PhoneNumber OnGet");
        await GetPhoneNumberFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("PhoneNumber OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitPhone();
        return RedirectToPage(
            Routes.Pages.Path.AuthorisedSignatoryDetailsPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("PhoneNumber OnPostSave");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitPhone();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    #region private methods
    private async Task SubmitPhone()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDTO tradeParty = GenerateDTO();
        TradePartyId = await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }

    private async Task GetPhoneNumberFromApiAsync()
    {
        TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            PhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
        }
    }

    private async Task GetIsAuthorisedSignatoryFromApiAsync()
    {
        TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            IsAuthorisedSignatory = tradeParty.Contact.IsAuthorisedSignatory;
        }
    }

    private TradePartyDTO GenerateDTO()
    {
        return new TradePartyDTO()
        {
            Id = TradePartyId,
            Contact = new TradeContactDTO()
            {
                TelephoneNumber = PhoneNumber,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };
    }
    #endregion
}
