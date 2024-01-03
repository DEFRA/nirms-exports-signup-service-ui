using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class SelfServeDashboardModel : BasePageModel<SelfServeDashboardModel>
{
    #region ui model variables
    [BindProperty]
    public Guid RegistrationID { get; set; }
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
    public string EstablishmentButtonText { get; set; } = "Add a place of dispatch";
    #endregion

    public SelfServeDashboardModel(
           ILogger<SelfServeDashboardModel> logger,
           ITraderService traderService,
           IEstablishmentService establishmentService,
           ICheckAnswersService checkAnswersService)
           : base(logger, traderService, establishmentService, checkAnswersService)
    { }
    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("OnGet Self serve dashboard");

        RegistrationID = Id;

        if (!_traderService.ValidateOrgId(User.Claims, RegistrationID).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        await PopulateModelPropertiesFromApi();

        if (Country == "NI")
        {
            EstablishmentButtonText = "Add a place of destination";
        }

        return Page();
    }

    private async Task PopulateModelPropertiesFromApi()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

        if (tradeParty == null || tradeParty?.Id == Guid.Empty)
        {
            return;
        }

        BusinessName = tradeParty?.PracticeName ?? string.Empty;
        RmsNumber = tradeParty?.RemosBusinessSchemeNumber ?? string.Empty;
        Country = tradeParty?.Address!.TradeCountry!;

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

    public IActionResult OnGetChangeContactDetails(Guid tradePartyId)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeUpdateContactPath,
            new { id = tradePartyId});
    }

    public IActionResult OnGetChangeAuthRepresentativeDetails(Guid tradePartyId)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeUpdateAuthRepPath,
            new { id = tradePartyId });
    }

    public IActionResult OnGetAddEstablishment(Guid tradePartyId, string countryChosen)
    {
        return RedirectToPage(
            Routes.Pages.Path.SelfServeEstablishmentHoldingPath,
            new { id = tradePartyId, country = countryChosen });
    }

}
