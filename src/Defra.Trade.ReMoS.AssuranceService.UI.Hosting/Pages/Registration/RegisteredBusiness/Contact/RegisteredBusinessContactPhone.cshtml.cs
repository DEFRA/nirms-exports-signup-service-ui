using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact
{
    public class RegisteredBusinessContactPhoneModel : PageModel
    {
        private readonly ITraderService _traderService;
        private readonly ILogger<RegisteredBusinessContactPhoneModel> _logger;

        [BindProperty]
        // This regex pattern supports various formats of UK phone numbers, including landlines and mobile numbers. It allows for optional spaces in different positions.
        [RegularExpression(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$", ErrorMessage = "Enter a telephone number in the correct format, like 01632 960 001, 07700 900 982 or +44 808 157 019")]        
        [Required(ErrorMessage = "Enter the phone number of the contact person")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        public Guid TradePartyId { get; set; }
        public bool? IsAuthorisedSignatory { get; set; }


        public RegisteredBusinessContactPhoneModel(ILogger<RegisteredBusinessContactPhoneModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TradePartyId = id;
            _logger.LogInformation("PhoneNumber OnGet");
            await GetPhoneNumberFromApiAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("PhoneNumber OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            await GetIsAuthorisedSignatoryFromApiAsync();
            TradePartyDTO tradeParty = GenerateDTO();
            TradePartyId = await _traderService.UpdateTradePartyContactAsync(tradeParty);


            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TradePartyId });
        }

        private async Task GetPhoneNumberFromApiAsync()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.Contact != null)
            {
                PhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
            }
        }

        private async Task GetIsAuthorisedSignatoryFromApiAsync()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.Contact != null)
            {
                IsAuthorisedSignatory = tradeParty.Contact.IsAuthorisedSignatory;
            }
        }

        private TradePartyDTO GenerateDTO()
        {
            return new TradePartyDTO()
            {
                Id = TradePartyId,
                Contact = new TradeContactDTO()
                {
                    TelephoneNumber = PhoneNumber,
                    IsAuthorisedSignatory = IsAuthorisedSignatory
                }
            };
        }
    }
}
