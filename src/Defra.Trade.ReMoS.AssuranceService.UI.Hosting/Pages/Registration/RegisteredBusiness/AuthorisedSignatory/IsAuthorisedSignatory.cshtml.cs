using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory;

public class IsAuthorisedSignatoryModel : BasePageModel<IsAuthorisedSignatoryModel>
{
    [BindProperty]
    public string? IsAuthorisedSignatory { get; set; } = null;

    [BindProperty]
    public string? BusinessName { get; set; }

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    public Guid ContactId { get; set; }

    [BindProperty]
    public Guid SignatoryId { get; set; }

    [BindProperty]
    public string? ContactName { get; set; } = string.Empty;

    public IsAuthorisedSignatoryModel(
        ITraderService traderService, 
        IEstablishmentService establishmentService, 
        ILogger<IsAuthorisedSignatoryModel> logger) : base(traderService, establishmentService, logger)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(IsAuthorisedSignatoryModel), nameof(OnGetAsync));

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

        _logger.LogInformation("IsAuthorisedSignatory onGet");
        var party = await GetIsAuthorisedSignatoryFromApiAsync();
        BusinessName = party?.PracticeName;
        ContactName = party?.Contact?.PersonName ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(IsAuthorisedSignatoryModel), nameof(OnPostSubmitAsync));

        ValidateAttribute();

        if (!ModelState.IsValid)
        {           
            return await OnGetAsync(OrgId);
        }

        await SubmitAuthSignatory();

        if (Convert.ToBoolean(IsAuthorisedSignatory))
        {
            var party = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false);
            string countryFlag = "GB";

            if (party?.Address?.TradeCountry == "NI")
            {
                countryFlag = "NI";
            }

            if (establishments != null && establishments.Any())
            {
                return RedirectToPage(
                    Routes.Pages.Path.AdditionalEstablishmentAddressPath,
                    new { id = OrgId, NI_GBFlag = countryFlag });
            }

            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeSearchPath,
                new { id = OrgId, NI_GBFlag = countryFlag });
        }

        return RedirectToPage(Routes.Pages.Path.AuthorisedSignatoryNamePath, new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(IsAuthorisedSignatoryModel), nameof(OnPostSaveAsync));

        ValidateAttribute();
        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitAuthSignatory();
        return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = OrgId });
    }

    #region private methods

    private void ValidateAttribute()
    {
        if (IsAuthorisedSignatory == string.Empty || IsAuthorisedSignatory == null)
        {
            ModelState.AddModelError(
                   nameof(IsAuthorisedSignatory),
                   $"Select 'yes' if {ContactName} is the authorised representative");
        }
    }

    private async Task SubmitAuthSignatory()
    {
        TradePartyDto tradeParty = await GenerateDTO();
        await _traderService.UpdateTradePartyAsync(tradeParty);

        var updatedTradeParty = await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

        IsAuthorisedSignatory ??= updatedTradeParty?.Contact?.IsAuthorisedSignatory.ToString();
    }

    private async Task<TradePartyDto?> GetIsAuthorisedSignatoryFromApiAsync()
    {
        var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        IsAuthorisedSignatory ??= tradeParty?.Contact?.IsAuthorisedSignatory.ToString();
        if (tradeParty != null && tradeParty.Contact != null && tradeParty.AuthorisedSignatory != null)
        {
            ContactId = tradeParty.Contact.Id;
            SignatoryId = tradeParty.AuthorisedSignatory.Id;
        }

        return tradeParty;
    }

    private async Task<TradePartyDto> GenerateDTO()
    {
        var tradeParty = await GetIsAuthorisedSignatoryFromApiAsync();
        var authorisedSignatory = tradeParty?.AuthorisedSignatory;

        //Checking if there was previously an authorised signatory assigned
        var previousState = tradeParty?.Contact?.IsAuthorisedSignatory;
        var isSignatory = Convert.ToBoolean(IsAuthorisedSignatory);

        var authorisedSignatoryStub = new AuthorisedSignatoryDto
        {
            Id = SignatoryId,
        };

        var tradeContactDto = new TradeContactDto
        {
            Id = ContactId,
            PersonName = tradeParty?.Contact?.PersonName,
            Email = tradeParty?.Contact?.Email,
            Position = tradeParty?.Contact?.Position,
            TelephoneNumber = tradeParty?.Contact?.TelephoneNumber,
            IsAuthorisedSignatory = isSignatory
        };

        if (tradeParty != null)
        {
            if (isSignatory)
            {
                authorisedSignatoryStub.Name = tradeParty.Contact?.PersonName;
                authorisedSignatoryStub.EmailAddress = tradeParty.Contact?.Email;
                authorisedSignatoryStub.Position = tradeParty.Contact?.Position;
                authorisedSignatoryStub.TradePartyId = TradePartyId;

                return new TradePartyDto()
                {
                    Id = TradePartyId,
                    ApprovalStatus = tradeParty.ApprovalStatus,
                    Contact = tradeContactDto,
                    AuthorisedSignatory = authorisedSignatoryStub
                };
            }
            else
            {
                if (previousState == true)
                {
                    authorisedSignatoryStub.TradePartyId = TradePartyId;

                    return new TradePartyDto()
                    {
                        Id = TradePartyId,
                        ApprovalStatus = tradeParty.ApprovalStatus,
                        Contact = tradeContactDto,
                        AuthorisedSignatory = authorisedSignatoryStub
                    };
                }
                else
                {
                    return new TradePartyDto()
                    {
                        Id = TradePartyId,
                        ApprovalStatus = tradeParty.ApprovalStatus,
                        Contact = tradeContactDto,
                        AuthorisedSignatory = authorisedSignatory
                    };
                }
            }
        }

        return new TradePartyDto()
        {
            Id = TradePartyId,
            Contact = new TradeContactDto()
            {
                Id = ContactId,
                IsAuthorisedSignatory = isSignatory
            },
            AuthorisedSignatory = authorisedSignatoryStub
        };
    }

    #endregion private methods
}