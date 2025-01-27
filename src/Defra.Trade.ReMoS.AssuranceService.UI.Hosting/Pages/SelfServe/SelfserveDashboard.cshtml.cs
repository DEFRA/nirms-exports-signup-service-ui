using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.CosmosDB.Fluent.Models;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System.Drawing.Printing;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class SelfServeDashboardModel : BasePageModel<SelfServeDashboardModel>
{
    #region ui model variables

    [BindProperty]
    public Guid TradePartyId { get; set; }

    [BindProperty]
    public Guid OrgId { get; set; }

    public string BusinessName { get; set; } = default!;
    public string RmsNumber { get; set; } = default!;
    public string ContactName { get; set; } = default!;
    public string ContactPosition { get; set; } = default!;
    public string ContactEmail { get; set; } = default!;
    public string ContactPhoneNumber { get; set; } = default!;
    public DateTime ContactSubmittedDate { get; set; } = default!;
    public DateTime ContactLastModifiedDate { get; set; } = default!;
    public string AuthSignatoryName { get; set; } = default!;
    public string AuthSignatoryPosition { get; set; } = default!;
    public string AuthSignatoryEmail { get; set; } = default!;
    public DateTime AuthSignatorySubmittedDate { get; set; } = default!;
    public DateTime AuthSignatoryLastModifiedDate { get; set; } = default!;

    [BindProperty]
    public string? SearchTerm { get; set; } = string.Empty;

    [BindProperty]
    public string? SortColumn { get; set; } = string.Empty;

    [BindProperty]
    public string? SortDirection { get; set; } = string.Empty;

    [BindProperty]
    public string EstablishmentButtonText { get; set; } = "dispatch";

    public int ApprovalStatus { get; set; }
    public PagedList<LogisticsLocationDto>? LogisticsLocations { get; set; } = new PagedList<LogisticsLocationDto>();
    public string? NI_GBFlag { get; set; } = string.Empty;

    #endregion ui model variables

    private readonly IFeatureManager _featureManager;

    public SelfServeDashboardModel(
           ILogger<SelfServeDashboardModel> logger,
           ITraderService traderService,
           IEstablishmentService establishmentService,
           ICheckAnswersService checkAnswersService,
           IFeatureManager featureManager)
           : base(logger, traderService, establishmentService, checkAnswersService)
    {
        _featureManager = featureManager;
    }

    public async Task<IActionResult> OnGetAsync(Guid Id, int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(SelfServeDashboardModel), nameof(OnGetAsync));

        OrgId = Id;
        TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;
        SearchTerm = searchTerm;
        SortColumn = sortColumn;
        SortDirection = sortDirection;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        await PopulateModelPropertiesFromApi();

        if (NI_GBFlag == "NI")
        {
            EstablishmentButtonText = "destination";
        }

        LogisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(
            TradePartyId,
            false,
            searchTerm?.ToLower(),
            sortColumn,
            sortDirection,
            NI_GBFlag,
            pageNumber,
            pageSize);

        return Page();
    }

    private async Task PopulateModelPropertiesFromApi()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

        if (tradeParty == null || tradeParty?.Id == Guid.Empty)
        {
            return;
        }

        BusinessName = tradeParty?.PracticeName ?? string.Empty;
        RmsNumber = tradeParty?.RemosBusinessSchemeNumber ?? string.Empty;
        NI_GBFlag = tradeParty?.Address?.TradeCountry == "NI" ? "NI" : "GB";
        ApprovalStatus = (int)(tradeParty?.ApprovalStatus!);

        if (tradeParty?.Contact != null)
        {
            ContactName = tradeParty.Contact.PersonName ?? string.Empty;
            ContactPosition = tradeParty.Contact.Position ?? string.Empty;
            ContactEmail = tradeParty.Contact.Email ?? string.Empty;
            ContactPhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
            ContactSubmittedDate = tradeParty.Contact.SubmittedDate;
            ContactLastModifiedDate = tradeParty.Contact.LastModifiedDate;
        }

        if (tradeParty?.AuthorisedSignatory != null)
        {
            AuthSignatoryName = tradeParty.AuthorisedSignatory.Name ?? string.Empty;
            AuthSignatoryPosition = tradeParty.AuthorisedSignatory.Position ?? string.Empty;
            AuthSignatoryEmail = tradeParty.AuthorisedSignatory.EmailAddress ?? string.Empty;
            AuthSignatorySubmittedDate = tradeParty.AuthorisedSignatory.SubmittedDate;
            AuthSignatoryLastModifiedDate = tradeParty.AuthorisedSignatory.LastModifiedDate;
        }
    }

    public IActionResult OnGetChangeContactDetails(Guid orgId)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeUpdateContactPath,
            new { id = orgId });
    }

    public IActionResult OnGetChangeAuthRepresentativeDetails(Guid orgId)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeUpdateAuthRepPath,
            new { id = orgId });
    }

    public async Task<IActionResult> OnGetAddEstablishment(Guid orgId, string NI_GBFlag)
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.SelfServeMvpPlus))
        {
            return RedirectToPage(
                Routes.Pages.Path.SelfServeEstablishmentPostcodeSearchPath,
                new { id = orgId, NI_GBFlag });
        }

        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentHoldingPath,
            new { id = orgId, NI_GBFlag });
    }

    public IActionResult OnPostSearchEstablishmentAsync()
    {
        if (SearchTerm == null && SortColumn != null)
        {
            return RedirectToPage(Routes.Pages.Path.SelfServeDashboardPath, "", new { id = OrgId, SearchTerm, SortColumn, SortDirection });
        }
        else
            return RedirectToPage(Routes.Pages.Path.SelfServeDashboardPath, "", new { id = OrgId, SearchTerm, SortColumn, SortDirection }, "filter");
    }

    public IActionResult OnPostShowAllEstablishments()
    {
        return RedirectToPage(Routes.Pages.Path.SelfServeDashboardPath, "", new { id = OrgId, SortColumn, SortDirection }, "filter");
    }

    public async Task<IActionResult> OnGetViewEstablishment(Guid orgId, Guid locationId, string NI_GBFlag, LogisticsLocationApprovalStatus status)
    {
        if (status == LogisticsLocationApprovalStatus.Draft)
        {
            return RedirectToPage(
                Routes.Pages.Path.SelfServeConfirmEstablishmentDetailsPath, new { id = orgId, locationId, NI_GBFlag });
        }
        else if ((status == LogisticsLocationApprovalStatus.Approved) || (status == LogisticsLocationApprovalStatus.Suspended) || (status == LogisticsLocationApprovalStatus.Removed))
        {
            return RedirectToPage(
                Routes.Pages.Path.SelfServeViewEstablishmentPath, new { id = orgId, locationId, NI_GBFlag });
        }
        else return await OnGetAsync(orgId, 1, 10, null);
    }

    public async Task<IActionResult> OnGetNavigateToPage(Guid orgId, int pageNumber, string? sortColumn = null, string? sortDirection = null, string? searchTerm = null)
    {
        return await OnGetAsync(orgId, pageNumber, 10, searchTerm, sortColumn, sortDirection);
    }
}