using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList
{
    public class RegistrationTaskListModel : BasePageModel<RegistrationTaskListModel>
    {
        #region ui model variables

        [BindProperty]
        public Guid TradePartyId { get; set; }

        [BindProperty]
        public Guid OrgId { get; set; }

        [BindProperty]
        public string SelectedBusinessName { get; set; } = default!;

        [BindProperty]
        public string EligibilityStatus { get; set; } = TaskListStatus.NOTSTART;

        public string PurposeOfBusinessStatus { get; set; } = TaskListStatus.NOTSTART;
        public string FboPhrStatus { get; set; } = TaskListStatus.NOTSTART;

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
        public int EstablishmentsCount { get; set; }

        #endregion ui model variables

        public RegistrationTaskListModel(
            ILogger<RegistrationTaskListModel> logger,
            ITraderService traderService,
            IEstablishmentService establishmentService,
            ICheckAnswersService checkAnswersService)
            : base(logger, traderService, establishmentService, checkAnswersService)
        { }

        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(RegistrationTaskListModel), nameof(OnGetAsync));

            OrgId = Id;
            var tradeParty = await _traderService.GetTradePartyByOrgIdAsync(OrgId);
            TradePartyId = tradeParty!.Id;

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(tradeParty))
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            tradeParty = await GetAPIData();

            if (_checkAnswersService.GetEligibilityProgress(tradeParty) != TaskListStatus.COMPLETE)
            {
                return RedirectToPage(
                        Routes.Pages.Path.RegisteredBusinessCountryPath,
                        new { id = OrgId });
            }

            return Page();
        }

        public async Task<TradePartyDto> GetAPIData()
        {
            TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            Country = tradeParty?.Address?.TradeCountry;

            if (tradeParty != null && tradeParty.Id != Guid.Empty)
            {
                EligibilityStatus = _checkAnswersService.GetEligibilityProgress(tradeParty);
                PurposeOfBusinessStatus = _checkAnswersService.GetPurposeOfBusinessProgress(tradeParty);
                FboPhrStatus = _checkAnswersService.GetFboPhrProgress(tradeParty);
                BusinessDetails = GetBusinessDetailsProgress(tradeParty!);
                ContactDetails = GetContactDetailsProgress(tradeParty!);
                AuthorisedSignatoryDetails = GetAuthorisedSignatoryProgress(tradeParty!);
                SelectedBusinessName = tradeParty.PracticeName ?? string.Empty;

                await EstablishmentsStatuses();
                CheckAnswersStatus();
            }
            return tradeParty!;
        }

        private async Task EstablishmentsStatuses()
        {
            var establishments = (await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId, false, string.Empty, string.Empty, string.Empty, string.Empty))?.Items;
            var gbEstablishments = establishments?.Where(x => x.NI_GBFlag == "GB");
            var niEstablishments = establishments?.Where(x => x.NI_GBFlag == "NI");

            if (gbEstablishments != null && gbEstablishments.Any())
                PlacesOfDispatch = TaskListStatus.COMPLETE;

            if (niEstablishments != null && niEstablishments.Any())
                PlacesOfDestination = TaskListStatus.COMPLETE;

            if (Country != "NI" && establishments != null && establishments!.Exists(x => x.NI_GBFlag == "GB"))
            {
                EstablishmentsCount = establishments!.Count(x => x.NI_GBFlag == "GB");
                EstablishmentsAdded = true;
            }

            if (Country == "NI" && establishments != null && establishments!.Exists(x => x.NI_GBFlag == "NI"))
            {
                EstablishmentsCount = establishments!.Count(x => x.NI_GBFlag == "NI");
                EstablishmentsAdded = true;
            }
        }

        private void CheckAnswersStatus()
        {
            if (EligibilityStatus == TaskListStatus.COMPLETE
                && BusinessDetails == TaskListStatus.COMPLETE
                && FboPhrStatus == TaskListStatus.COMPLETE
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