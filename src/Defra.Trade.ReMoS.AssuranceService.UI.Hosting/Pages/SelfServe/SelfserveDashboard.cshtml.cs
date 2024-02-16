using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System.Diagnostics.CodeAnalysis;

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
    public string Country { get; set; } = default!;
    public string EstablishmentButtonText { get; set; } = "dispatch";
    public int ApprovalStatus { get; set; }
    #endregion

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

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("OnGet Self serve dashboard");

        OrgId = Id;
        TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;

        if (!_traderService.ValidateOrgId(User.Claims, OrgId))
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        await PopulateModelPropertiesFromApi();

        if (Country == "NI")
        {
            EstablishmentButtonText = "destination";
        }

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
        Country = tradeParty?.Address!.TradeCountry!;
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

    [ExcludeFromCodeCoverage]
    public async Task<IActionResult> OnGetAddEstablishment(Guid orgId, string countryChosen)
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.SelfServeMvpPlus))
        {
            return RedirectToPage(
                Routes.Pages.Path.SelfServeEstablishmentPostcodeSearchPath,
                new { id = orgId, country = countryChosen });
        }

        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentHoldingPath,
            new { id = orgId, country = countryChosen });
    }

}
