using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class EstablishmentNameAndAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'.,_/(),]*$", ErrorMessage = "Enter establishment name using only letters, numbers, commas, brackets, full stops, underscores, forward slashes, hyphens, apostrophes or ampersands")]
    [StringLength(100, ErrorMessage = "Establishment name must be 100 characters or less")]
    [Required(ErrorMessage = "Enter an establishment name")]
    public string EstablishmentName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/(),]*$", ErrorMessage = "Enter address line 1 using only letters, numbers, commas, brackets, full stops, underscores, forward slashes, hyphens, apostrophes or ampersands")]
    [StringLength(100, ErrorMessage = "Address line 1 must be 100 characters or less")]
    [Required(ErrorMessage = "Enter address line 1")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/(),]*$", ErrorMessage = "Enter address line 2 using only letters, numbers, commas, brackets, full stops, underscores, forward slashes, hyphens, apostrophes or ampersands")]
    [StringLength(100, ErrorMessage = "Address line 2 must be 100 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/(),]*$", ErrorMessage = "Enter a town or city using only letters, numbers, commas, brackets, full stops, underscores, forward slashes, hyphens, apostrophes or ampersands")]
    [StringLength(100, ErrorMessage = "Town or city must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a town or city")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/(),]*$", ErrorMessage = "Enter a county using only letters, numbers, commas, brackets, full stops, underscores, forward slashes, hyphens, apostrophes or ampersands")]
    [StringLength(100, ErrorMessage = "County must be 100 characters or less")]
    public string? County { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})", ErrorMessage = "Enter a real postcode" )]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a postcode")]
    public string PostCode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public Guid? EstablishmentId { get; set; }
    [BindProperty]
    public string? Uprn { get; set; }

    public string? ContentHeading { get; set; } = string.Empty;

    public string? ContentText { get; set; } = string.Empty;
    public string? ContextHint { get; set; } = string.Empty;

    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;
    #endregion

    private readonly ILogger<EstablishmentNameAndAddressModel> _logger;
    private readonly IEstablishmentService _establishmentService;
    private readonly ITraderService _traderService;

    public EstablishmentNameAndAddressModel(
        ILogger<EstablishmentNameAndAddressModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
    }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid? establishmentId, string? uprn, string? NI_GBFlag = "GB")
    {
        _logger.LogInformation("Establishment manual address OnGet");
        TradePartyId = id;
        this.NI_GBFlag = NI_GBFlag;
        EstablishmentId = establishmentId;
        Uprn = uprn;

        if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (_traderService.IsTradePartySignedUp(TradePartyId).Result)
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        await RetrieveEstablishmentDetails();

        if (NI_GBFlag == "NI")
        {
            ContextHint = "If your place of destination belongs to a different business";
            ContentHeading = "Add a place of destination";
        }
        else
        {
            ContextHint = "If your place of dispatch belongs to a different business";
            ContentHeading = "Add a place of dispatch";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Establishment manual address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(TradePartyId, EstablishmentId, Uprn, NI_GBFlag ?? string.Empty);
        }


        if(await CheckForDuplicateAsync())
        {
            string place;
            if (NI_GBFlag == "NI")
            {
                place = "destination";
            }
            else
            {
                place = "dispatch";
            }

            var baseError = $"This address has already been added as a place of {place} - enter a different address";

            ModelState.AddModelError(nameof(EstablishmentName), baseError);
            return await OnGetAsync(TradePartyId, EstablishmentId, Uprn, NI_GBFlag ?? string.Empty);
        }
        

        var establishmentId = await SaveEstablishmentDetails();

        return RedirectToPage(
            Routes.Pages.Path.EstablishmentContactEmailPath, 
            new { id = TradePartyId, locationId = establishmentId, NI_GBFlag });

    }

    public async Task<Guid?> SaveEstablishmentDetails()
    {
        var establishmentDto = new LogisticsLocationDto() { Address = new TradeAddressDto()};

        if (EstablishmentId != Guid.Empty && EstablishmentId != null)
        {
            establishmentDto = await _establishmentService.GetEstablishmentByIdAsync((Guid)EstablishmentId);
        }

        establishmentDto!.Name = EstablishmentName;
        establishmentDto.Address!.LineOne = LineOne;
        establishmentDto.Address.LineTwo = LineTwo;
        establishmentDto.Address.County = County;
        establishmentDto.Address.CityName = CityName;
        establishmentDto.Address.PostCode = PostCode;
        establishmentDto.NI_GBFlag = NI_GBFlag;

        if (EstablishmentId == Guid.Empty || Uprn != null || EstablishmentId == null) 
        {
            return await _establishmentService.CreateEstablishmentForTradePartyAsync(TradePartyId, establishmentDto);
        }
        else
        {
            await _establishmentService.UpdateEstablishmentDetailsAsync(establishmentDto);
            return EstablishmentId;
        }
    }

    public async Task RetrieveEstablishmentDetails()
    {
        LogisticsLocationDto establishment = new();
        if (Uprn != null)
        {
            establishment = await _establishmentService.GetLogisticsLocationByUprnAsync(Uprn);
        }
        else
        {
            if (EstablishmentId != Guid.Empty && EstablishmentId != null)
            {
                establishment = await _establishmentService.GetEstablishmentByIdAsync((Guid)EstablishmentId!) ?? new LogisticsLocationDto();
            }
        }
        EstablishmentName = establishment?.Name ?? string.Empty;
        LineOne = establishment?.Address?.LineOne ?? string.Empty;
        LineTwo = establishment?.Address?.LineTwo ?? string.Empty;
        CityName = establishment?.Address?.CityName ?? string.Empty;
        County = establishment?.Address?.County ?? string.Empty;
        PostCode = establishment?.Address?.PostCode ?? string.Empty;
       
    }

    public async Task<bool> CheckForDuplicateAsync()
    {
        var existingEstablishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId);

        var duplicates = existingEstablishments!.Where(x => x.Name!.ToUpper() == EstablishmentName.ToUpper()
        && x.Address!.LineOne!.ToUpper() == LineOne.ToUpper()
        && x.Address!.PostCode!.Replace(" ", "").ToUpper() == PostCode.Replace(" ", "").ToUpper()
        && x.Id != EstablishmentId);

        if (duplicates.Any())
        {
            return true;
        }
        return false;
    }
}
