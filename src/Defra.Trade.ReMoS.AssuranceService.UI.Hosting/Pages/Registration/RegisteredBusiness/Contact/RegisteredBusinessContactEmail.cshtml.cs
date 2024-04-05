using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;

public class RegisteredBusinessContactEmailModel : BasePageModel<RegisteredBusinessContactEmailModel>
{
    #region UI Model
    [BindProperty]
    [StringLengthMaximum(100, ErrorMessage = "The email address cannot be longer than 100 characters")]
    [Required(ErrorMessage = "Enter an email address")]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid ContactId { get; set; }
    [BindProperty]
    public string? PracticeName { get; set; }
    [BindProperty]
    public string? ContactName { get; set; }
    public bool? IsAuthorisedSignatory { get; set; }
    public Guid AuthorisedSignatoryId { get; set; }
    public TradePartyDto TradePartyDto { get; set; } = new TradePartyDto();
    public TradePartyDto? TradePartyDtoCurrent { get; set; } = new TradePartyDto();
    #endregion

    public RegisteredBusinessContactEmailModel(
    ILogger<RegisteredBusinessContactEmailModel> logger, 
        ITraderService traderService) : base(traderService, logger)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactEmailModel), nameof(OnGetAsync));

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

        _logger.LogInformation("Email OnGet");

        await GetTradePartyFromApiAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactEmailModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitEmail();
        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactPhonePath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(RegisteredBusinessContactEmailModel), nameof(OnPostSaveAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitEmail();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    #region private methods
    private async Task SubmitEmail()
    {
        await GetIsAuthorisedSignatoryFromApiAsync();
        TradePartyDto = await GenerateDTOAsync();
        await _traderService.UpdateTradePartyContactAsync(TradePartyDto);
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
        TradePartyDtoCurrent = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (TradePartyDtoCurrent != null && TradePartyDtoCurrent.Contact != null)
        {
            IsAuthorisedSignatory = TradePartyDtoCurrent.Contact.IsAuthorisedSignatory;
            if (TradePartyDtoCurrent!.AuthorisedSignatory != null)
            {
                AuthorisedSignatoryId = TradePartyDtoCurrent.AuthorisedSignatory.Id;
            }
        }
    }

    private async Task<TradePartyDto> GenerateDTOAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        var tradePartyDto = new TradePartyDto()
        {
            Id = TradePartyId,
            ApprovalStatus = tradeParty!.ApprovalStatus,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                Email = Email,
                IsAuthorisedSignatory = IsAuthorisedSignatory
            }
        };

        if (IsAuthorisedSignatory == true)
        {
            tradePartyDto.AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = AuthorisedSignatoryId,
                EmailAddress = Email,
                Name = TradePartyDtoCurrent!.Contact!.PersonName,
                Position = TradePartyDtoCurrent.Contact.Position
            };
        }

        return tradePartyDto;
    }
    #endregion
}
