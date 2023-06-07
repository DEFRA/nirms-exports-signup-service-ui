using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
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
    public List<SelectListItem> LogisticsLocationsList { get; set; } = default!;

    [BindProperty]
    public string SelectedLogisticsLocation { get; set; } = default!;

    public string? ContentHeading { get; set; } = string.Empty;
    
    public string? ContentText { get; set; } = string.Empty;
    
    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;
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

    public async Task<IActionResult> OnGetAsync(Guid id, string postcode, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Postcode result OnGetAsync");
        Postcode = postcode;
        TradePartyId= id;

        var LogisticsLocations = new List<LogisticsLocationDTO>();
        this.NI_GBFlag = NI_GBFlag;

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a point of destination (optional)";
            ContentText = "Add all establishments in Northern Ireland where your goods go after the port of entry. For example, a hub or store.";
        }
        else
        {
            ContentHeading = "Add a point of departure";
            ContentText = "Add all establishments in Great Britan from which your goods will be departing under the scheme.";
        }

        
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
            Routes.Pages.Path.EstablishmentContactEmailPath,
            new { id = TradePartyId, locationId = Guid.Parse(SelectedLogisticsLocation), NI_GBFlag });
    }
}
