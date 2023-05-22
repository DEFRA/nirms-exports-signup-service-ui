using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
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
        public Guid TraderId { get; set; }

        public RegisteredBusinessContactPhoneModel(ILogger<RegisteredBusinessContactPhoneModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync(Guid? id = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            TraderId = (TraderId != Guid.Empty) ? TraderId : id ?? Guid.Empty;
            _logger.LogInformation("PhoneNumber OnGet");

            if (TraderId != Guid.Empty)
            {
                await GetPhoneNumberFromApiAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("PhoneNumber OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            TradePartyDTO tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId) ?? new TradePartyDTO();
            tradeParty.Contact ??= new TradeContactDTO();

            tradeParty.Contact.TelephoneNumber = PhoneNumber;


            if (tradeParty.Id == Guid.Empty)
            {
                TraderId = await _traderService.CreateTradePartyAsync(tradeParty);
            }
            else
            {
                await _traderService.UpdateTradePartyAsync(tradeParty);
            }

            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TraderId });
        }

        private async Task GetPhoneNumberFromApiAsync()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
            if (tradeParty != null && tradeParty.Contact != null)
            {
                PhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
            }
        }
    }
}
