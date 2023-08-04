using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList
{
    public class RegistrationTaskListModel : PageModel
    {
        #region ui model variables
        [BindProperty]
        public Guid RegistrationID { get; set; }
        [BindProperty]
        public string EligibilityStatus { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string BusinessDetails { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ContactDetails { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string AuthorisedSignatoryDetails { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string PlacesOfDispatch { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string PlacesOfDestination { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ReviewAnswers { get; set; } = TaskListStatus.CANNOTSTART;
        public string? Country { get; set; }
        public bool EstablishmentsAdded { get; set; }
        #endregion

        private readonly ILogger<RegistrationTaskListModel> _logger;
        private readonly ITraderService _traderService;
        private readonly IEstablishmentService _establishmentService;
        private readonly ICheckAnswersService _checkAnswersService;

        public RegistrationTaskListModel(ILogger<RegistrationTaskListModel> logger, ITraderService traderService, IEstablishmentService establishmentService, ICheckAnswersService checkAnswersService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
            _checkAnswersService = checkAnswersService ?? throw new ArgumentNullException(nameof(checkAnswersService));
        }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            _logger.LogInformation("OnGet");

            RegistrationID = Id;

            TradePartyDto tradeParty = await GetAPIData();

            if (_checkAnswersService.GetEligibilityProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return RedirectToPage(
                        Routes.Pages.Path.RegisteredBusinessCountryPath,
                        new { id = RegistrationID });
            }

            return Page();
        }

        public async Task<TradePartyDto> GetAPIData()
        {
            TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);
            Country = tradeParty?.Address?.TradeCountry;

            if (tradeParty != null && tradeParty.Id != Guid.Empty)
            {
                EligibilityStatus = _checkAnswersService.GetEligibilityProgress(tradeParty);

                BusinessDetails = GetBusinessDetailsProgress(tradeParty!);
                ContactDetails = GetContactDetailsProgress(tradeParty!);
                AuthorisedSignatoryDetails = GetAuthorisedSignatoryProgress(tradeParty!);
                
                await EstablishmentsStatuses();
                CheckAnswersStatus();
            }
            return tradeParty;
        }

        private async Task EstablishmentsStatuses()
        {
            var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID);
            var gbEstablishments = establishments?.Where(x => x.NI_GBFlag == "GB");
            var niEstablishments = establishments?.Where(x => x.NI_GBFlag == "NI");

            if (gbEstablishments != null && gbEstablishments.Any())
                PlacesOfDispatch = TaskListStatus.COMPLETE;

            if (niEstablishments != null && niEstablishments.Any())
                PlacesOfDestination = TaskListStatus.COMPLETE;

            if (Country != "NI" && establishments != null && establishments!.Any(x => x.NI_GBFlag == "GB"))
            {
                EstablishmentsAdded = true;
            }

            if (Country == "NI" && establishments != null && establishments!.Any(x => x.NI_GBFlag == "NI"))
            {
                EstablishmentsAdded = true;
            }

        }

        private void CheckAnswersStatus()
        {
            if (EligibilityStatus == TaskListStatus.COMPLETE
                && BusinessDetails == TaskListStatus.COMPLETE
                && ContactDetails == TaskListStatus.COMPLETE
                && AuthorisedSignatoryDetails == TaskListStatus.COMPLETE
                && ((PlacesOfDispatch == TaskListStatus.COMPLETE && Country != "NI") || (PlacesOfDestination == TaskListStatus.COMPLETE && Country == "NI")))
            {
                ReviewAnswers = TaskListStatus.NOTSTART;
            }
            else
            {
                ReviewAnswers = TaskListStatus.CANNOTSTART;
            }
        }

        public string GetBusinessDetailsProgress(TradePartyDto tradeParty)
        {
            return _checkAnswersService.GetBusinessDetailsProgress(tradeParty);
        }

        public string GetContactDetailsProgress(TradePartyDto tradeParty)
        {
            return _checkAnswersService.GetContactDetailsProgress(tradeParty);
        }

        public string GetAuthorisedSignatoryProgress(TradePartyDto tradeParty)
        {
            return _checkAnswersService.GetAuthorisedSignatoryProgress(tradeParty);
        }
    }
}
