using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.BatchAI.Fluent.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class AdditionalEstablishmentAddressModel : PageModel
{
    #region ui model variables
    public string? AddAddressesComplete { get; set; } = string.Empty;
    public List<LogisticsLocationDto>? LogisticsLocations { get; set; } = new List<LogisticsLocationDto>();
    public Guid TradePartyId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<AdditionalEstablishmentAddressModel> _logger;
    private readonly IEstablishmentService _establishmentService;
    private readonly ITraderService _traderService;
    private readonly ICheckAnswersService _checkAnswersService;

    public AdditionalEstablishmentAddressModel(
        ILogger<AdditionalEstablishmentAddressModel> logger,
        IEstablishmentService establishmentService, 
        ITraderService traderService,
        ICheckAnswersService checkAnswersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        _checkAnswersService = checkAnswersService ?? throw new ArgumentNullException(nameof(checkAnswersService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Additional establishment manual address OnGet");
        TradePartyId = id;
        this.NI_GBFlag = NI_GBFlag;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
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

        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId))?
            .Where(x => x.NI_GBFlag == this.NI_GBFlag)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Additional establishment manual address OnPostSubmit");

        if (String.IsNullOrWhiteSpace(AddAddressesComplete))
        {
            var baseError = "Select if you have added all your places of ";
            var errorMessage = NI_GBFlag == "NI" ? $"{baseError}destination" : $"{baseError}dispatch";
            ModelState.AddModelError(nameof(AddAddressesComplete), errorMessage);
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, NI_GBFlag!);
        }

        if (AddAddressesComplete != null && AddAddressesComplete.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeSearchPath, 
                new { id = TradePartyId, NI_GBFlag });
        }

        if (await ReadyForCheckAnswersAsync())
        {
           return RedirectToPage(
            Routes.Pages.Path.RegistrationCheckYourAnswersPath,
            new { id = TradePartyId });
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath, 
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Additional establishment manual address OnPostSave");

        if (String.IsNullOrWhiteSpace(AddAddressesComplete))
        {
            var baseError = "Select no if you want to add another place of ";
            var errorMessage = NI_GBFlag == "NI" ? $"{baseError}destination" : $"{baseError}dispatch";
            ModelState.AddModelError(nameof(AddAddressesComplete), errorMessage);
        }

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, NI_GBFlag!);
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TradePartyId });
    }

    public async Task<IActionResult> OnGetRemoveEstablishment(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        await _establishmentService.RemoveEstablishmentAsync(establishmentId);
        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId))?.ToList();

        if (LogisticsLocations?.Count > 0)
            return await OnGetAsync(tradePartyId, NI_GBFlag);
        else
        {
            return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = tradePartyId });
        }
    }

    public IActionResult OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
    {

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentNameAndAddressPath,
            new { id = tradePartyId, establishmentId, NI_GBFlag });

    }

    public IActionResult OnGetChangeEmail(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
    {
        return RedirectToPage(
            Routes.Pages.Path.EstablishmentContactEmailPath,
            new { id = tradePartyId, locationId = establishmentId, NI_GBFlag });
    }

   private async Task<bool> ReadyForCheckAnswersAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        if (tradeParty == null)
        {
            return false;
        }

        if (_checkAnswersService.GetBusinessDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
        {
            return false;
        }

        if (_checkAnswersService.GetAuthorisedSignatoryProgress(tradeParty) != TaskListStatus.COMPLETE)
        {
            return false;
        }

        if (_checkAnswersService.GetContactDetailsProgress(tradeParty) != TaskListStatus.COMPLETE)
        {
            return false;
        }

        return true;
    }
}
