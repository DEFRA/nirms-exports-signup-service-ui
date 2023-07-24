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

        public RegistrationTaskListModel(ILogger<RegistrationTaskListModel> logger, ITraderService traderService, IEstablishmentService establishmentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
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

            await GetAPIData();

            return Page();
        }

        public async Task GetAPIData()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);
            Country = tradeParty?.Address?.TradeCountry;

            if (tradeParty != null && tradeParty.Id != Guid.Empty)
            {
                if (tradeParty?.Address != null)
                {
                    if (tradeParty.Address.TradeCountry != null && !string.IsNullOrEmpty(tradeParty.FboNumber))
                        EligibilityStatus = TaskListStatus.COMPLETE;
                }

                BusinessDetails = GetBusinessDetailsProgress(tradeParty!);
                ContactDetails = GetContactDetailsProgress(tradeParty!);
                AuthorisedSignatoryDetails = GetAuthorisedSignatoryProgress(tradeParty!);
                
                await EstablishmentsStatuses();
                CheckAnswersStatus();
            }
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

        public string GetBusinessDetailsProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.PartyName != null && tradeParty.Address != null
                && tradeParty.Address.LineOne != null && tradeParty.Address.PostCode != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty?.PartyName != null || tradeParty?.Address != null
                || tradeParty?.Address?.LineOne != null || tradeParty?.Address?.PostCode != null)
            {
                return TaskListStatus.INPROGRESS;                
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetContactDetailsProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.Contact != null && tradeParty.Contact.PersonName != null && tradeParty.Contact.Email != null && tradeParty.Contact.TelephoneNumber != null && tradeParty.Contact.Position != null)
            {
                return TaskListStatus.COMPLETE;
            }

            if (tradeParty?.Contact?.PersonName != null || tradeParty?.Contact?.Email != null || tradeParty?.Contact?.TelephoneNumber != null || tradeParty?.Contact?.Position != null)
            {
                return TaskListStatus.INPROGRESS;
            }

            return TaskListStatus.NOTSTART;
        }

        public string GetAuthorisedSignatoryProgress(TradePartyDTO tradeParty)
        {
            if (tradeParty.AuthorisedSignatory != null && tradeParty.Contact != null)
            {
                if (tradeParty.Contact?.IsAuthorisedSignatory == true)
                {
                    return TaskListStatus.COMPLETE;
                }
                else
                {
                    if (tradeParty.AuthorisedSignatory.Name != null && tradeParty.AuthorisedSignatory.Position != null && tradeParty.AuthorisedSignatory.EmailAddress != null)
                    {
                        return TaskListStatus.COMPLETE;
                    }
                    else
                    {
                        return TaskListStatus.INPROGRESS;
                    }
                }
            }

            return TaskListStatus.NOTSTART;
        }
    }
}
