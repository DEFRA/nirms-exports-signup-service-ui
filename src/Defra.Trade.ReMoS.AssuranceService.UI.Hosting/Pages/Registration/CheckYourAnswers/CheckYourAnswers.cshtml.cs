using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers
{
    public class CheckYourAnswersModel : BasePageModel<CheckYourAnswersModel>
    {
        #region ui model variables

        [BindProperty]
        public Guid RegistrationID { get; set; }

        public string? ContentHeading { get; set; } = string.Empty;
        public string? ContentText { get; set; } = string.Empty;
        public string NI_GBFlag { get; set; } = string.Empty;
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
            _logger.LogInformation("OnGet");

            RegistrationID = Id;

            if (RegistrationID == Guid.Empty)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = RegistrationID });
            }

            if (!_traderService.ValidateOrgId(User.Claims, RegistrationID).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(RegistrationID).Result)
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            TradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

            NI_GBFlag = TradeParty?.Address?.TradeCountry == "NI" ? "NI" : "GB";

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID))?
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

        public async Task<IActionResult> OnGetRemoveEstablishment(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            var logisticsLocation = await _establishmentService.GetEstablishmentByIdAsync(establishmentId);
            logisticsLocation!.IsRemoved = true;
            await _establishmentService.UpdateEstablishmentDetailsAsync(logisticsLocation);

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId))?
                .OrderBy(x => x.CreatedDate)
                .ToList();

            if (LogisticsLocations?.Count > 0)
                return await OnGetAsync(tradePartyId);
            else
                return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = tradePartyId, NI_GBFlag });
        }

        public IActionResult OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath,
                new { id = tradePartyId, establishmentId, NI_GBFlag });
        }

        public IActionResult OnGetChangeEmail(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentContactEmailPath,
                new { id = tradePartyId, locationId = establishmentId, NI_GBFlag });
        }

        public async Task<IActionResult> OnPostSubmitAsync(Guid RegistrationID)
        {
            TradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

            var logisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID);

            if (_checkAnswersService.ReadyForCheckAnswers(TradeParty!) &&
                logisticsLocations != null &&
                _checkAnswersService.IsLogisticsLocationsDataPresent(TradeParty!, logisticsLocations))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTermsAndConditionsPath,
                    new { id = RegistrationID });
            }
            else
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                    new { id = RegistrationID });
            }
        }
    }
}