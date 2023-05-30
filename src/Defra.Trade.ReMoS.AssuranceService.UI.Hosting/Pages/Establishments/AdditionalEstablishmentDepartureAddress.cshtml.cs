using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

[BindProperties]
public class AdditionalEstablishmentDepartureAddressModel : PageModel
{
    #region ui model variables
    [Required(ErrorMessage = "Select yes if you want to add another point of departure")]
    public string AdditionalAddress { get; set; } = string.Empty;
    public List<LogisticsLocationDetailsDTO>? LogisticsLocations { get; set; } = new List<LogisticsLocationDetailsDTO>();
    public Guid TradePartyId { get; set; }
    #endregion

    private readonly ILogger<AdditionalEstablishmentDepartureAddressModel> _logger;
    private readonly IEstablishmentService _establishmentService;

    public AdditionalEstablishmentDepartureAddressModel(
        ILogger<AdditionalEstablishmentDepartureAddressModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        _logger.LogInformation("Additional establishment manual address OnGet");
        TradePartyId = id;

        //retrieve all departure establishments with addresses for this trade party
        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId))?.ToList();

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
            return RedirectToPage(Routes.Pages.Path.EstablishmentDeparturePostcodeSearchPath, new { id = TradePartyId });
        }
        else return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = TradePartyId });
    }

    public async Task<IActionResult> OnGetRemoveEstablishment(Guid tradePartyId, Guid establishmentId)
    {
        await _establishmentService.RemoveEstablishmentFromPartyAsync(tradePartyId, establishmentId);
        LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId))?.ToList();

        if (LogisticsLocations?.Count > 0)
            return await OnGetAsync(tradePartyId);
        else
            return RedirectToPage(Routes.Pages.Path.EstablishmentDeparturePostcodeSearchPath, new { id = tradePartyId });

    }

    public async Task<IActionResult> OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId)
    {
        bool establishmentAddedManually = await _establishmentService.IsFirstTradePartyForEstablishment(tradePartyId, establishmentId);
        await _establishmentService.RemoveEstablishmentFromPartyAsync(tradePartyId, establishmentId);

        if (establishmentAddedManually)
        {   
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentDepartureAddressPath,
                new { id = tradePartyId });
        }

        return RedirectToPage(Routes.Pages.Path.EstablishmentDeparturePostcodeSearchPath, new { id = tradePartyId });
    }
}
