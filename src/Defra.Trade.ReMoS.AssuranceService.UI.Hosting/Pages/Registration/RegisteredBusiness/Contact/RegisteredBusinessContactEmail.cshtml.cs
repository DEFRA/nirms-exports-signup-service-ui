using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;

public class RegisteredBusinessContactEmailModel : PageModel
{
    #region UI Model
    [BindProperty]
    [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [StringLength(100, ErrorMessage = "Email is too long")]
    [Required(ErrorMessage = "Enter the email address of the contact person")]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid ContactId { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
    #endregion

    private readonly ITraderService _traderService;
    private readonly ILogger<RegisteredBusinessContactEmailModel> _logger;

    public RegisteredBusinessContactEmailModel(
        ILogger<RegisteredBusinessContactEmailModel> logger, 
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;
        _logger.LogInformation("Email OnGet");

        await GetEmailAddressFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {            
        _logger.LogInformation("Email OnPostSubmit");
        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitEmail();
        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactPhonePath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Email OnPostSave");
        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        await SubmitEmail();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    #region private methods
    private async Task SubmitEmail()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDTO tradeParty = GenerateDTO();
        await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }

    private async Task GetEmailAddressFromApiAsync()
    {
        TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Email = tradeParty.Contact.Email ?? string.Empty;
            ContactId = tradeParty.Contact.Id;
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
                Id = ContactId,
                Email = Email,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };
    }
    #endregion
}
