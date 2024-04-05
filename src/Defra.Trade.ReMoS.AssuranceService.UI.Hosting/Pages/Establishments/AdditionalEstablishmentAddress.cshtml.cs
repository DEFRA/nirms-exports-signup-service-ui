using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class AdditionalEstablishmentAddressModel : BasePageModel<AdditionalEstablishmentAddressModel>
{
    #region ui model variables

    public string? AddAddressesComplete { get; set; } = string.Empty;
    public List<LogisticsLocationDto>? LogisticsLocations { get; set; } = new List<LogisticsLocationDto>();
    public Guid TradePartyId { get; set; }
    public Guid OrgId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? NI_GBFlag { get; set; } = string.Empty;
    public string? PracticeName { get; set; } = string.Empty;

    #endregion ui model variables

    public AdditionalEstablishmentAddressModel(
        ILogger<AdditionalEstablishmentAddressModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService,
        ICheckAnswersService checkAnswersService) : base(logger, traderService, establishmentService, checkAnswersService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnGetAsync));

        OrgId = id;
        this.NI_GBFlag = NI_GBFlag;

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

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Places of destination";
            ContentText = "destination";
        }
        else
        {
            ContentHeading = "Places of dispatch";
            ContentText = "dispatch";
        }

        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false))?
            .Where(x => x.NI_GBFlag == this.NI_GBFlag)
            .OrderBy(x => x.CreatedDate)
            .ToList();

        PracticeName = (await _traderService.GetTradePartyByIdAsync(TradePartyId))?.PracticeName ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnPostSubmitAsync));

        if (String.IsNullOrWhiteSpace(AddAddressesComplete))
        {
            var baseError = "Select if you have added all your places of ";
            var errorMessage = NI_GBFlag == "NI" ? $"{baseError}destination" : $"{baseError}dispatch";
            ModelState.AddModelError(nameof(AddAddressesComplete), errorMessage);
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId, NI_GBFlag!);
        }

        if (AddAddressesComplete != null && AddAddressesComplete.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeSearchPath, 
                new { id = OrgId, NI_GBFlag });
        }
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        if (_checkAnswersService.ReadyForCheckAnswers(tradeParty!))
        {
            return RedirectToPage(
             Routes.Pages.Path.RegistrationCheckYourAnswersPath,
             new { id = OrgId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnPostSaveAsync));

        if (String.IsNullOrWhiteSpace(AddAddressesComplete))
        {
            var baseError = "Select if you have added all your places of ";
            var errorMessage = NI_GBFlag == "NI" ? $"{baseError}destination" : $"{baseError}dispatch";
            ModelState.AddModelError(nameof(AddAddressesComplete), errorMessage);
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId, NI_GBFlag!);
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = OrgId });
    }

    public async Task<IActionResult> OnGetRemoveEstablishment(Guid orgId, Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnGetRemoveEstablishment));

        var logisticsLocation = await _establishmentService.GetEstablishmentByIdAsync(establishmentId);
        logisticsLocation!.IsRemoved = true;
        await _establishmentService.UpdateEstablishmentDetailsAsync(logisticsLocation);

        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId, false))?.ToList();

        if (LogisticsLocations?.Count > 0)
            return await OnGetAsync(orgId, NI_GBFlag);
        else
        {
            return RedirectToPage(Routes.Pages.Path.EstablishmentPostcodeSearchPath, new { id = orgId, NI_GBFlag });
        }
    }

    public IActionResult OnGetChangeEstablishmentAddress(Guid orgId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnGetChangeEstablishmentAddress));

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentNameAndAddressPath,
            new { id = orgId, establishmentId, NI_GBFlag });
    }

    public IActionResult OnGetChangeEmail(Guid orgId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(AdditionalEstablishmentAddressModel), nameof(OnGetChangeEmail));

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentContactEmailPath,
            new { id = orgId, locationId = establishmentId, NI_GBFlag });
    }
}