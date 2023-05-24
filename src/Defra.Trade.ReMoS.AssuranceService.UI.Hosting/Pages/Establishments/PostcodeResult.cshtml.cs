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

        var LogisticsLocations = new List<LogisticsLocation>();

        if (Postcode != string.Empty)
        {
            LogisticsLocations = await _establishmentService.GetLogisticsLocationByPostcodeAsync(Postcode);

            //LogisticsLocations.Add(new LogisticsLocation()
            //{
            //    Name = "Test 2",
            //    Id = Guid.NewGuid(),
            //    Address = new TradeAddress()
            //    {
            //        LineOne = "line 1",
            //        CityName = "city",
            //        PostCode = Postcode,
            //    }
            //});
            //LogisticsLocations.Add(new LogisticsLocation()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Test 1",
            //    Address = new TradeAddress()
            //    {
            //        LineOne = "line 1",
            //        CityName = "city",
            //        PostCode = Postcode,
            //    }
            //});
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

        var logisticsLocationRelationshipDTO = new LogisticsLocationRelationshipDTO()
        {
            TraderId = TradePartyId,
            EstablishmentId = Guid.Parse(SelectedLogisticsLocation)
        };

        await _establishmentService.AddLogisticsLocationRelationshipAsync(logisticsLocationRelationshipDTO);

        return RedirectToPage(
            Routes.Pages.Path.AdditionalEstablishmentDepartureAddressPath,
            new { id = TradePartyId });
    }
}
