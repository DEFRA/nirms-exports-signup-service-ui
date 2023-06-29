using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class AdditionalEstablishmentAddressModel : PageModel
{
    #region ui model variables
    [Required(ErrorMessage = "Select yes if you want to add another point of departure")]
    public string AdditionalAddress { get; set; } = string.Empty;
    public List<LogisticsLocationDTO>? LogisticsLocations { get; set; } = new List<LogisticsLocationDTO>();
    public Guid TradePartyId { get; set; }
    public string? ContentHeading { get; set; } = string.Empty;
    public string? ContentText { get; set; } = string.Empty;
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<AdditionalEstablishmentAddressModel> _logger;
    private readonly IEstablishmentService _establishmentService;

    public AdditionalEstablishmentAddressModel(
        ILogger<AdditionalEstablishmentAddressModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Additional establishment manual address OnGet");
        TradePartyId = id;
        this.NI_GBFlag = NI_GBFlag;

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Points of destination (optional)";
            ContentText = "destination";
        }
        else
        {
            ContentHeading = "Points of departure";
            ContentText = "departure";
        }

        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId))?
            .Where(x => x.NI_GBFlag == this.NI_GBFlag)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Additional establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        if (AdditionalAddress == "yes")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath, 
                new { id = TradePartyId, NI_GBFlag });
        }
        else return RedirectToPage(
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

    public async Task<IActionResult> OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
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
}
