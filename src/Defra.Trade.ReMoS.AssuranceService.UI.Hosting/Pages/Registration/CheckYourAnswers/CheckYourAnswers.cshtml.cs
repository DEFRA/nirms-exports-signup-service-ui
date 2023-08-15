using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#pragma warning disable CS1998

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers
{
    public class CheckYourAnswersModel : PageModel
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
        #endregion

        private readonly ILogger<CheckYourAnswersModel> _logger;
        private readonly IEstablishmentService _establishmentService;
        private readonly ITraderService _traderService;

        public CheckYourAnswersModel(ILogger<CheckYourAnswersModel> logger, IEstablishmentService establishmentService, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        }

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

            TradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

            NI_GBFlag = TradeParty?.Address?.TradeCountry == "NI" ? "NI" : "GB";

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID))?
                .Where(x => x.NI_GBFlag == this.NI_GBFlag)
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
            await _establishmentService.RemoveEstablishmentAsync(establishmentId);
            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId))?.ToList();

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


        public async Task<IActionResult> OnPostSubmitAsync()
        {
            return RedirectToPage(
                Routes.Pages.Path.RegistrationTermsAndConditionsPath,
                new { id = RegistrationID });
        }

    }
}
