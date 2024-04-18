using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers
{
    public class CheckYourAnswersModel : BasePageModel<CheckYourAnswersModel>
    {
        #region ui model variables

        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid OrgId { get; set; }

        public string? ContentHeading { get; set; } = string.Empty;
        public string? ContentText { get; set; } = string.Empty;
        public string NI_GBFlag { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public List<LogisticsLocationDto>? LogisticsLocations { get; set; } = new List<LogisticsLocationDto>();

        [BindProperty]
        public TradePartyDto? TradeParty { get; set; } = new TradePartyDto();

        #endregion ui model variables

        public CheckYourAnswersModel(
            ILogger<CheckYourAnswersModel> logger, 
            IEstablishmentService establishmentService, 
            ITraderService traderService, 
            ICheckAnswersService checkAnswersService) : base(logger, traderService, establishmentService, checkAnswersService)
        {}

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(CheckYourAnswersModel), nameof(OnGetAsync));

            OrgId = Id;
            var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
            TradePartyId = tradeParty!.Id;

            if (TradePartyId == Guid.Empty)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = OrgId });
            }

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(tradeParty))
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            TradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            NI_GBFlag = TradeParty?.Address?.TradeCountry == "NI" ? "NI" : "GB";
            Purpose = TradeParty?.Address?.TradeCountry == "NI" ? "Receive Consignments" : "Send Consignments";

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false))?
                .Where(x => x.NI_GBFlag == this.NI_GBFlag)
                .OrderBy(x => x.CreatedDate)
                .ToList();

            if (NI_GBFlag == "NI")
            {
                ContentHeading = "Places of destination";
                ContentText = "destination";
            }
            else
            {
                ContentHeading = "Places of dispatch";
                ContentText = "dispatch";
            }

            return Page();
        }

        public async Task<IActionResult> OnGetRemoveEstablishment(Guid orgId, Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            var logisticsLocation = await _establishmentService.GetEstablishmentByIdAsync(establishmentId);
            logisticsLocation!.IsRemoved = true;
            await _establishmentService.UpdateEstablishmentDetailsAsync(logisticsLocation);

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId, false))?
                .OrderBy(x => x.CreatedDate)
                .ToList();

            if (LogisticsLocations?.Count > 0)
                return await OnGetAsync(orgId);
            else
                return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = orgId, NI_GBFlag });
        }

        public IActionResult OnGetChangeEstablishmentAddress(Guid orgId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath,
                new { id = orgId, establishmentId, NI_GBFlag });
        }

        public IActionResult OnGetChangeEmail(Guid orgId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentContactEmailPath,
                new { id = orgId, locationId = establishmentId, NI_GBFlag });
        }

        public async Task<IActionResult> OnPostSubmitAsync(Guid OrgId)
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(CheckYourAnswersModel), nameof(OnPostSubmitAsync));

            TradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            var logisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false);

            if (_checkAnswersService.ReadyForCheckAnswers(TradeParty!) &&
                logisticsLocations != null &&
                _checkAnswersService.IsLogisticsLocationsDataPresent(TradeParty!, logisticsLocations))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTermsAndConditionsPath,
                    new { id = OrgId });
            }
            else
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = OrgId });
            }
        }
    }
}