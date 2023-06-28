using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers
{
    public class CheckYourAnswersModel : PageModel
    {
        #region ui model variables
        [BindProperty]
        public Guid RegistrationID { get; set; }
        public string? ContentHeading { get; set; } = string.Empty;
        public string? ContentText { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string NI_GBFlag { get; set; } = string.Empty;
        public List<LogisticsLocationDetailsDTO>? LogisticsLocations { get; set; } = new List<LogisticsLocationDetailsDTO>();
        #endregion

        private readonly ILogger<CheckYourAnswersModel> _logger;
        private readonly IEstablishmentService _establishmentService;

        public CheckYourAnswersModel(ILogger<CheckYourAnswersModel> logger, IEstablishmentService establishmentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        }

        public async Task<IActionResult> OnGetAsync(Guid Id, string Country)
        {
            _logger.LogInformation("OnGet");

            RegistrationID = Id;
            this.Country = Country;

            if (RegistrationID == Guid.Empty)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegisteredBusinessCountryPath,
                    new { id = RegistrationID });
            }

            NI_GBFlag = Country == "NI" ? "NI" : "GB";

            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID))?
                .Where(x => x.NI_GBFlag == this.NI_GBFlag)
                .ToList();

            if (NI_GBFlag == "NI")
            {
                ContentHeading = "Points of destination";
                ContentText = "destination";
            }
            else
            {
                ContentHeading = "Points of departure";
                ContentText = "departure";
            }

            return Page();
        }

        public async Task<IActionResult> OnGetRemoveEstablishment(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            await _establishmentService.RemoveEstablishmentFromPartyAsync(tradePartyId, establishmentId);
            LogisticsLocations = (await _establishmentService.GetEstablishmentsForTradePartyAsync(tradePartyId))?.ToList();

            if (LogisticsLocations?.Count > 0)
                return await OnGetAsync(tradePartyId, Country);
            else
                return RedirectToPage(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = tradePartyId, NI_GBFlag });
        }

        public async Task<IActionResult> OnGetChangeEstablishmentAddress(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            bool establishmentAddedManually = await _establishmentService.IsFirstTradePartyForEstablishment(tradePartyId, establishmentId);

            if (establishmentAddedManually)
            {
                return RedirectToPage(
                    Routes.Pages.Path.EstablishmentNameAndAddressPath,
                    new { id = tradePartyId, establishmentId, NI_GBFlag });
            }

            return await OnGetAsync(tradePartyId, Country);
        }

        public IActionResult OnGetChangeEmail(Guid tradePartyId, Guid establishmentId, string NI_GBFlag = "GB")
        {
            return RedirectToPage(
                Routes.Pages.Path.EstablishmentContactEmailPath,
                new { id = tradePartyId, locationId = establishmentId, NI_GBFlag });
        }

    }
}
