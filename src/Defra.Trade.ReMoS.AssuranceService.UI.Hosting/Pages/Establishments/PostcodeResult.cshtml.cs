using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class PostcodeResultModel : PageModel
{
    #region UI Models
    [BindProperty]
    public string? Postcode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public List<SelectListItem> LogisticsLocationsList { get; set; }

    [BindProperty]
    public string SelectedLogisticsLocation {get; set; }
    #endregion

    private readonly ILogger<PostcodeResultModel> _logger;
    private readonly IEstablishmentService _establishmentService;

    public PostcodeResultModel(
        ILogger<PostcodeResultModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id, string postcode)
    {
        _logger.LogInformation("Postcode result OnGetAsync");
        Postcode = postcode;
        TradePartyId= id;

        var LogisticsLocations = new List<LogisticsLocationDTO>();

        if (Postcode != string.Empty)
        {
            LogisticsLocations = await _establishmentService.GetEstablishmentByPostcodeAsync(Postcode);

        }

        LogisticsLocationsList = LogisticsLocations.Select(x => new SelectListItem { Text = $"{x.Name}, {x.Address?.LineOne}, {x.Address?.CityName}, {x.Address?.PostCode}", Value = x.Id.ToString() }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("PostcodeResult OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, Postcode);
        }

        var logisticsLocationRelationshipDTO = new LogisticsLocationBusinessRelationshipDTO()
        {
            TradePartyId = TradePartyId,
            LogisticsLocationId = Guid.Parse(SelectedLogisticsLocation)
        };

        await _establishmentService.AddEstablishmentToPartyAsync(logisticsLocationRelationshipDTO);

        return RedirectToPage(
            Routes.Pages.Path.AdditionalEstablishmentDepartureAddressPath,
            new { id = TradePartyId });
    }
}
