using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class EstablishmentNameAndAddressModel : PageModel
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

    public string? ContentHeading { get; set; } = string.Empty;

    public string? ContentText { get; set; } = string.Empty;

    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<EstablishmentNameAndAddressModel> _logger;
    private readonly IEstablishmentService _establishmentService;

    public EstablishmentNameAndAddressModel(
        ILogger<EstablishmentNameAndAddressModel> logger,
        IEstablishmentService establishmentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid? establishmentId, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Establishment manual address OnGet");
        TradePartyId = id;
        this.NI_GBFlag = NI_GBFlag;
        EstablishmentId = establishmentId ?? Guid.Empty;

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a point of destination";
        }
        else
        {
            ContentHeading = "Add a point of departure";
        }

        await RetrieveEstablishmentDetails();
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, EstablishmentId, NI_GBFlag);
        }

        var establishmentId = await SaveEstablishmentDetails();

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentContactEmailPath, 
            new { id = TradePartyId, locationId = establishmentId, NI_GBFlag });

    }

    private async Task<Guid?> SaveEstablishmentDetails()
    {
        //TODO - if existing estab, update, else create

        if (EstablishmentId == Guid.Empty) 
        {
            //Create establishment
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
                },
                NI_GBFlag = NI_GBFlag,
            };
            return await _establishmentService.CreateEstablishmentAndAddToPartyAsync(TradePartyId, establishmentDto);
        }
        else
        {
            //Update establishment
            var establishment = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId);
            establishment.Name = EstablishmentName;
            establishment.Address.LineOne = LineOne;
            establishment.Address.LineTwo = LineTwo;
            establishment.Address.CityName = CityName;
            establishment.Address.TradeCountry = Country;
            establishment.Address.PostCode = PostCode;
        }
    }

    private async Task RetrieveEstablishmentDetails()
    {

        LogisticsLocationDTO? establishment = await _establishmentService.GetEstablishmentByIdAsync(EstablishmentId) ?? new LogisticsLocationDTO();

        EstablishmentName = establishment?.Name ?? string.Empty;
        LineOne = establishment?.Address?.LineOne ?? string.Empty;
        LineTwo = establishment?.Address?.LineTwo ?? string.Empty;
        CityName = establishment?.Address?.CityName ?? string.Empty;
        Country = establishment?.Address?.TradeCountry ?? string.Empty;
        PostCode = establishment?.Address?.PostCode ?? string.Empty;

    }
}
