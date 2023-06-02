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
        public string BusinessName { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string NatureOfBusiness { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string RegisteredAddress { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ContactFullName { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ContactPosition { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ContactEmailAddress { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ContactTelephone { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string PointsOfDeparture { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string PointsOfDestination { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string SPSGoodsCategories { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string SPSAssuranceCommitment { get; set; } = TaskListStatus.NOTSTART;
        [BindProperty]
        public string ReviewAnswers { get; set; } = TaskListStatus.CANNOTSTART;
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
            await GetAPIData();

            return Page();
        }

        public async Task GetAPIData()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(RegistrationID);

            if (tradeParty != null && tradeParty.Id != Guid.Empty)
            {
                if (tradeParty.PartyName != null)
                    BusinessName = TaskListStatus.COMPLETE;
                if (tradeParty.NatureOfBusiness != null)
                    NatureOfBusiness = TaskListStatus.COMPLETE;

                if (tradeParty.Address != null)
                {
                    if (tradeParty.Address.TradeCountry != null)
                        EligibilityStatus = TaskListStatus.COMPLETE;
                    if (tradeParty.Address.LineOne != null && tradeParty.Address.PostCode != null)
                        RegisteredAddress = TaskListStatus.COMPLETE;
                }

                if (tradeParty.Contact != null)
                {
                    if (tradeParty.Contact.PersonName != null)
                        ContactFullName = TaskListStatus.COMPLETE;
                    if (tradeParty.Contact.Email != null)
                        ContactEmailAddress = TaskListStatus.COMPLETE;
                    if (tradeParty.Contact.TelephoneNumber != null)
                        ContactTelephone = TaskListStatus.COMPLETE;
                    if (tradeParty.Contact.Position != null)
                        ContactPosition = TaskListStatus.COMPLETE;
                }

                //TODO - replace this call with a simple 'do establishments exist' functionality
                var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(RegistrationID);
                var gbEstablishments = establishments?.Where(x => x.NI_GBFlag == "GB");
                var niEstablishments = establishments?.Where(x => x.NI_GBFlag == "NI");

                if (gbEstablishments != null && gbEstablishments.Count() > 0)
                    PointsOfDeparture = TaskListStatus.COMPLETE;

                if (niEstablishments != null && niEstablishments.Count() > 0)
                    PointsOfDestination = TaskListStatus.COMPLETE;
            }
        }
    }
}
