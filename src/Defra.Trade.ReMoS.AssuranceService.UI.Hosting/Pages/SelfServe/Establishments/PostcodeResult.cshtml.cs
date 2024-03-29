using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServeMvpPlus)]
public class PostcodeResultModel : BasePageModel<PostcodeResultModel>
{
    #region UI Models
    [BindProperty]
    public string? Postcode { get; set; } = string.Empty;

    [BindProperty]
    public Guid TradePartyId { get; set; }
    
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    public List<SelectListItem>? EstablishmentsList { get; set; } = default!;

    [BindProperty]
    public string SelectedEstablishment { get; set; } = default!;
    
    public string? BusinessName { get; set; }

    public string? ContentHeading { get; set; } = string.Empty;

    public string? ContentText { get; set; } = string.Empty;

    [BindProperty]
    public string? NI_GBFlag { get; set; } = string.Empty;

    [BindProperty]
    public bool IsSubmitDisabled { get; set; } = false;
    #endregion

    public PostcodeResultModel(
      ILogger<PostcodeResultModel> logger,
      IEstablishmentService establishmentService,
      ITraderService traderService) : base(logger, traderService, establishmentService)
    { }

    public async Task<IActionResult> OnGetAsync(Guid id, string postcode, string NI_GBFlag = "GB")
    {
        _logger.LogInformation("Postcode result OnGetAsync");
        Postcode = postcode;
        OrgId = id;
        this.NI_GBFlag = NI_GBFlag;

        var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
        TradePartyId = tradeParty!.Id;

        BusinessName = BusinessName = await _traderService.GetBusinessNameAsync(TradePartyId);

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
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

        var EstablishmentsApi = new List<AddressDto>();

        if (Postcode != string.Empty)
        {
            EstablishmentsApi = await _establishmentService.GetTradeAddressApiByPostcodeAsync(Postcode);
        }

        var EstablishmentsApiList = EstablishmentsApi != null ? EstablishmentsApi
            .Select(x => new SelectListItem
            {
                Text = $"{x.Address}",
                Value = x.Uprn
            })
            .ToList() : Enumerable.Empty<SelectListItem>();

        EstablishmentsList = new[] { EstablishmentsApiList! }.SelectMany(x => x).ToList();

        if (EstablishmentsList == null || EstablishmentsList.Count == 0)
        {
            return RedirectToPage(Routes.Pages.Path.SelfServeEstablishmentPostcodeNoResultPath, new { id = OrgId, NI_GBFlag, postcode = Postcode });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("PostcodeResult OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId, Postcode!, NI_GBFlag!);
        }

        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentNameAndAddressPath,
            new { id = OrgId, uprn = SelectedEstablishment, NI_GBFlag });


    }
}
