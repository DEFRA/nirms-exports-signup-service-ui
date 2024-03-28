using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

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

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }
        if (!GetType().FullName!.Contains("SelfServe") && _traderService.IsTradePartySignedUp(tradeParty))
        {
            return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }

        if (NI_GBFlag == "NI")
        {
            ContentHeading = "Add a place of destination";
            ContentText = "These are the establishments that consignments will go to in Northern Ireland after the port of entry under the scheme.";
        }
        else
        {
            ContentHeading = "Add a place of dispatch";
            ContentText = "These are the establishments that consignments to Northern Ireland will depart from under the scheme.";
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
            if (GetType().FullName!.Contains("SelfServe"))
                return RedirectToPage(Routes.Pages.Path.SelfServeEstablishmentPostcodeNoResultPath, new { id = OrgId, NI_GBFlag, postcode = Postcode });
            else
                return RedirectToPage(Routes.Pages.Path.PostcodeNoResultPath, new { id = OrgId, NI_GBFlag, postcode = Postcode });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("PostcodeResult OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId, Postcode!);
        }

        if (GetType().FullName!.Contains("SelfServe"))
            return RedirectToPage(Routes.Pages.Path.SelfServeEstablishmentNameAndAddressPath,
                new { id = OrgId, uprn = SelectedEstablishment, NI_GBFlag });
        else
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath,
                new { id = OrgId, uprn = SelectedEstablishment, NI_GBFlag });


    }
}
