using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;

public class RegisteredBusinessContactEmailModel : BasePageModel<RegisteredBusinessContactEmailModel>
{
    #region UI Model
    [BindProperty]
    [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    [Required(ErrorMessage = "Enter an email address")]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid ContactId { get; set; }
    [BindProperty]
    public string? PracticeName { get; set; }
    [BindProperty]
    public string? ContactName { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
    #endregion
    
    public RegisteredBusinessContactEmailModel(
    ILogger<RegisteredBusinessContactEmailModel> logger, 
        ITraderService traderService) : base(traderService, logger)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        TradePartyId = id;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (_traderService.IsTradePartySignedUp(id).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        _logger.LogInformation("Email OnGet");

        await GetTradePartyFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {            
        _logger.LogInformation("Email OnPostSubmit");

        if (!IsInputValid())
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

        if (!IsInputValid())
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
        TradePartyDto tradeParty = GenerateDTO();
        await _traderService.UpdateTradePartyContactAsync(tradeParty);
    }

    private async Task GetTradePartyFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Email = tradeParty.Contact.Email ?? string.Empty;
            ContactId = tradeParty.Contact.Id;
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

    private TradePartyDto GenerateDTO()
    {
        return new TradePartyDto()
        {
            Id = TradePartyId,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                Email = Email,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };
    }

    private bool IsInputValid()
    {
        if (Email != null && Email.Length > 100)
            ModelState.AddModelError(nameof(Email), "The email address cannot be longer than 100 characters");

        if (!ModelState.IsValid || ModelState.ErrorCount > 0)
            return false;

        return true;
    }
    #endregion
}
