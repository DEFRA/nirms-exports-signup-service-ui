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

        public RegisteredBusinessContactPhoneModel(ILogger<RegisteredBusinessContactPhoneModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogInformation("PhoneNumber OnGet");
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("PhoneNumber OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            var tradeParty = new TradePartyDTO
            {
                Contact = new TradeContactDTO { TelephoneNumber = PhoneNumber },
            };

            await _traderService.UpdateTradePartyAsync(tradeParty);

            return Redirect(Routes.RegistrationTasklist);
        }
    }
}
