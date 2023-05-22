using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact
{
    public class RegisteredBusinessContactEmailModel : PageModel
    {
        [BindProperty]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [StringLength(100, ErrorMessage = "Email is too long")]
        [Required(ErrorMessage = "Enter the email address of the contact person")]
        public string Email { get; set; } = string.Empty;

        private readonly ITraderService _traderService;
        private readonly ILogger<RegisteredBusinessContactEmailModel> _logger;

        [BindProperty]
        public Guid TraderId { get; set; }
        public RegisteredBusinessContactEmailModel(ILogger<RegisteredBusinessContactEmailModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync(Guid? id = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            TraderId = (TraderId != Guid.Empty) ? TraderId : id ?? Guid.Empty;
            _logger.LogInformation("Email OnGet");

            if (TraderId != Guid.Empty)
            {
                await GetEmailAddressFromApiAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {            
            _logger.LogInformation("Email OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId) ?? new TradePartyDTO();
            tradeParty.Contact ??= new TradeContactDTO();

            tradeParty.Contact.EmailAddress = Email;


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

        private async Task GetEmailAddressFromApiAsync()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
            if (tradeParty != null && tradeParty.Contact != null)
            {
                Email = tradeParty.Contact.EmailAddress ?? string.Empty;
            }
        }
    }
}
