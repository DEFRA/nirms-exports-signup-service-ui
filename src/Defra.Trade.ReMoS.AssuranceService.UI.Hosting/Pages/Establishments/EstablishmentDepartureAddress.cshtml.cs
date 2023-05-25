using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class EstablishmentDepartureAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter establishment name using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Establishment name must be 100 characters or less")]
    [Required(ErrorMessage = "Enter establishment name.")]
    public string EstablishmentName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 1 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 1 must be 100 characters or less")]
    [Required(ErrorMessage = "Enter address line 1.")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 2 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 2 must be 100 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a town or city using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Town or city must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a town or city.")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a country using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Country must be 100 characters or less")]
    public string? Country { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter a real postcode.")]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a post code.")]
    public string PostCode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public Guid EstablishmentId { get; set; }
    #endregion

    private readonly ILogger<EstablishmentDepartureAddressModel> _logger;
    //private readonly ITraderService _traderService;
    private readonly IEstablishmentService _establishmentService;

    public EstablishmentDepartureAddressModel(
        ILogger<EstablishmentDepartureAddressModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IActionResult> OnGetAsync(Guid id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _logger.LogInformation("Establishment manual address OnGet");
        TradePartyId = id;

        await RetrieveEstablishmentDetails();
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId);
        }

        var establishmentId = await SaveEstablishmentDetails();

        return RedirectToPage(Routes.Pages.Path.AdditionalEstablishmentDepartureAddressPath, new { id = TradePartyId });
    }

    private async Task<Guid?> SaveEstablishmentDetails()
    {
        var establishmentDto = new LogisticsLocationDTO
        {
            Id = EstablishmentId,
            Name = EstablishmentName,
            Address = new TradeAddressDTO
            {
                LineOne = LineOne,
                LineTwo = LineTwo,
                CityName = CityName,
                TradeCountry = Country,
                PostCode = PostCode,
            }
        };
        return await _establishmentService.CreateEstablishmentAndAddToPartyAsync(TradePartyId, establishmentDto);
    }

    private async Task RetrieveEstablishmentDetails()
    {

        LogisticsLocationDTO? establishment = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId) ?? null;

        //TODO - this needs implementing
        //if (establishment == null)
        //{
        //    IEnumerable<LogisticsLocationDTO>? establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId);
        //    establishment = establishments?.FirstOrDefault();
        //}

        EstablishmentName = establishment?.Name ?? string.Empty;
        LineOne = establishment?.Address?.LineOne ?? string.Empty;
        LineTwo = establishment?.Address?.LineTwo ?? string.Empty;
        CityName = establishment?.Address?.CityName ?? string.Empty;
        Country = establishment?.Address?.TradeCountry ?? string.Empty;
        PostCode = establishment?.Address?.PostCode ?? string.Empty;

    }
}
