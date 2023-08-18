using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class PostcodeResultModel : PageModel
{
    #region UI Models
    [BindProperty]
    public string? Postcode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public List<SelectListItem> EstablishmentsList { get; set; } = default!;

    [BindProperty]
    public string SelectedEstablishment { get; set; } = default!;

    public string? ContentHeading { get; set; } = string.Empty;
    
    public string? ContentText { get; set; } = string.Empty;
    
    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;
    [BindProperty]
    public bool IsSubmitDisabled { get; set; } = false; 
    #endregion

    private readonly ILogger<PostcodeResultModel> _logger;
    private readonly IEstablishmentService _establishmentService;
    private readonly ITraderService _traderService;

    public PostcodeResultModel(
        ILogger<PostcodeResultModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, string postcode, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Postcode result OnGetAsync");
        Postcode = postcode;
        TradePartyId= id;

        var establishments = new List<LogisticsLocationDto>();
        this.NI_GBFlag = NI_GBFlag;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a place of destination";
            ContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
        }
        else
        {
            ContentHeading = "Add a place of dispatch";
            ContentText = "The locations which are part of your business that consignments to Northern Ireland will depart from under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
        }

        if (Postcode != string.Empty)
        {
            establishments = await _establishmentService.GetEstablishmentByPostcodeAsync(Postcode);
        }

        EstablishmentsList = establishments?.Count > 0 ? 
            establishments
            .Where(x => x.NI_GBFlag == NI_GBFlag)
            .Select(x => new SelectListItem { Text = $"{x.Name}, {x.Address?.LineOne}, {x.Address?.CityName}, {x.Address?.PostCode}", Value = x.Id.ToString() })
            .ToList() 
            : null!;

        if (EstablishmentsList == null || EstablishmentsList.Count == 0)
        {
            ModelState.AddModelError(nameof(EstablishmentsList), "No search results returned");
            IsSubmitDisabled = true;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("PostcodeResult OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, Postcode!);
        }

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentContactEmailPath,
            new { id = TradePartyId, locationId = Guid.Parse(SelectedEstablishment), NI_GBFlag });
    }
}
