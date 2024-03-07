using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8602, CS8601

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class AuthorisedSignatoryEmailModel : BasePageModel<AuthorisedSignatoryEmailModel>
{
    #region ui model
    [StringLengthMaximum(100, ErrorMessage = "The email address cannot be longer than 100 characters")]
    [BindProperty]
    [Required(ErrorMessage = "Enter an email address")]
    public string? Email { get; set; }
    [BindProperty]
    public string? BusinessName { get; set; }
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }
    [BindProperty]
    public Guid SignatoryId { get; set; }
    [BindProperty]
    public string? Country { get; set; }
    public string? Name { get; set; } = string.Empty;
    #endregion

    public AuthorisedSignatoryEmailModel(
        ITraderService traderService,
        IEstablishmentService establishmentService,
        ILogger<AuthorisedSignatoryEmailModel> logger) : base(traderService, establishmentService, logger)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
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

        var party = await GetSignatoryEmailFromApiAsync();
        BusinessName = party?.PracticeName;
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Signatory Email OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitEmail();

        string countryFlag = "GB";

        if (Country == "NI")
        {
            countryFlag = "NI";
        }

        var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false);

        if ( establishments != null && establishments.Any())
        {
            return RedirectToPage(
                Routes.Pages.Path.AdditionalEstablishmentAddressPath,
                new { id = OrgId, NI_GBFlag = countryFlag });
        }

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentPostcodeSearchPath,
            new { id = OrgId, NI_GBFlag = countryFlag });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Signatory Email OnPostSave");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitEmail();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    private async Task SubmitEmail()
    {
        var tradeParty = await GenerateDTO();
        await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);
    }

    private async Task<TradePartyDto?> GetSignatoryEmailFromApiAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
        {
            SignatoryId = tradeParty.AuthorisedSignatory.Id;
            Email = string.IsNullOrEmpty(Email) ? tradeParty.AuthorisedSignatory.EmailAddress ?? "" : Email;
            Country = tradeParty.Address.TradeCountry;
            Name = tradeParty.AuthorisedSignatory.Name;
            return tradeParty;
        }
        return null;
    }

    private async Task<TradePartyDto> GenerateDTO()
    {
        var tradeParty = await GetSignatoryEmailFromApiAsync();
        return new TradePartyDto()
        {
            Id = TradePartyId,
            ApprovalStatus = tradeParty!.ApprovalStatus,
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Id = SignatoryId,
                Name = tradeParty?.AuthorisedSignatory?.Name,
                Position = tradeParty?.AuthorisedSignatory?.Position,
                EmailAddress = Email
            }
        };
    }
}
