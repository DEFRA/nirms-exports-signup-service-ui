using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class EstablishmentNameAndAddressModel : BasePageModel<EstablishmentNameAndAddressModel>
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter an establishment name using only letters, numbers, brackets, full stops, hyphens, underscores, forward slashes, apostrophes or ampersands")]
    [Required(ErrorMessage = "Enter an establishment name")]
    [StringLengthMaximum(100, ErrorMessage = "Establishment name must be 100 characters or less")]
    public string EstablishmentName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter address line 1 using only letters, numbers, brackets, full stops, hyphens, underscores, forward slashes, apostrophes or ampersands")]
    [Required(ErrorMessage = "Enter address line 1")]
    [StringLengthMaximum(50, ErrorMessage = "Address line 1 must be 50 characters or less")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter address line 2 using only letters, numbers, brackets, full stops, hyphens, underscores, forward slashes, apostrophes or ampersands")]
    [StringLengthMaximum(50, ErrorMessage = "Address line 2 must be 50 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter a town or city using only letters, numbers, brackets, full stops, hyphens, underscores, forward slashes, apostrophes or ampersands")]
    [Required(ErrorMessage = "Enter a town or city")]
    [StringLengthMaximum(100, ErrorMessage = "Town or city must be 100 characters or less")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter a county using only letters, numbers, brackets, full stops, hyphens, underscores, forward slashes, apostrophes or ampersands")]
    [StringLengthMaximum(100, ErrorMessage = "County must be 100 characters or less")]
    public string? County { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})", ErrorMessage = "Enter a real postcode")]
    [Required(ErrorMessage = "Enter a postcode")]
    [StringLengthMaximum(100, ErrorMessage = "Post code must be 100 characters or less")]
    public string PostCode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    public Guid? EstablishmentId { get; set; }
    [BindProperty]
    public string? Uprn { get; set; }

    public string? ContentHeading { get; set; } = string.Empty;

    public string? ContentText { get; set; } = string.Empty;

    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;
    [BindProperty]
    public string? BackPostcode { get; set; }
    #endregion

    public EstablishmentNameAndAddressModel(
        ILogger<EstablishmentNameAndAddressModel> logger,
        IEstablishmentService establishmentService,
        ITraderService traderService) : base(logger, traderService, establishmentService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid? establishmentId, string? uprn, string? backPostcode, string? NI_GBFlag = "GB")
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(EstablishmentNameAndAddressModel), nameof(OnGetAsync));

        OrgId = id;
        this.NI_GBFlag = NI_GBFlag;
        EstablishmentId = establishmentId;
        Uprn = uprn;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (!GetType().FullName!.Contains("SelfServe") && _traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        await RetrieveEstablishmentDetails();

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a place of destination";
        }
        else
        {
            ContentHeading = "Add a place of dispatch";
        }

        ViewData["Title"] = ContentHeading;
        BackPostcode = PostCode != string.Empty ? PostCode : backPostcode;

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(EstablishmentNameAndAddressModel), nameof(OnPostSubmitAsync));

        if (!IsInputValid() || !IsPostCodeValid())
        {
            return await OnGetAsync(OrgId, EstablishmentId, Uprn, BackPostcode, NI_GBFlag ?? string.Empty);
        }

        Guid? establishmentId;
        try
        {
            establishmentId = await _establishmentService.SaveEstablishmentDetails(
                EstablishmentId,
                TradePartyId,
                new LogisticsLocationDto
                {
                    Name = EstablishmentName,
                    ApprovalStatus = LogisticsLocationApprovalStatus.Draft,
                    Address = new TradeAddressDto { LineOne = LineOne, LineTwo = LineTwo, County = County, CityName = CityName, PostCode = PostCode },
                    LastModifiedDate = DateTime.UtcNow
                },
                NI_GBFlag??string.Empty,
                Uprn
            );
        }
        catch (BadHttpRequestException)
        {
            ModelState.AddModelError(nameof(EstablishmentName), GenerateDuplicateError());
            return await OnGetAsync(OrgId, EstablishmentId, Uprn, BackPostcode, NI_GBFlag ?? string.Empty);
        }

        if (GetType().FullName!.Contains("SelfServe"))
            return RedirectToPage(
                Routes.Pages.Path.SelfServeEstablishmentContactEmailPath,
                new { id = OrgId, locationId = establishmentId, NI_GBFlag });
        else
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentContactEmailPath,
                new { id = OrgId, locationId = establishmentId, NI_GBFlag });
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

    public virtual bool IsInputValid()
    {
        if (!ModelState.IsValid)
            return false;

        return true;
    }

    private bool IsPostCodeValid()
    {
        if (PostCode!.ToUpper().StartsWith("BT") && (NI_GBFlag == "GB"))
            ModelState.AddModelError(nameof(PostCode), "Enter a postcode in England, Scotland or Wales");

        if (!PostCode!.ToUpper().StartsWith("BT") && (NI_GBFlag == "NI"))
            ModelState.AddModelError(nameof(PostCode), "Enter a postcode in Northern Ireland");

        if (ModelState.ErrorCount > 0)
            return false;

        return true;
    }

    private string GenerateDuplicateError()
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

        return $"This address has already been added as a place of {place} - enter a different address";
    }

}
