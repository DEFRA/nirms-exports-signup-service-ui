using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    public class RegisteredBusinessNameModel : PageModel
    {
        private readonly ITraderService _traderService;
        private readonly ILogger<RegisteredBusinessNameModel> _logger;

        #region UI Model
        [BindProperty]
        [Required(ErrorMessage = "Enter your business name")]
        [RegularExpression(@"^[a-zA-Z0-9\s-_./()&]*$", ErrorMessage = "Enter your business name using only letters, numbers, and special characters -_./()&")]
        [MaxLength(100, ErrorMessage = "Business name is too long")]
        public string? Name { get; set; } = string.Empty;
        [BindProperty]
        public Guid TradePartyId { get; set; }
        #endregion

        public RegisteredBusinessNameModel(ILogger<RegisteredBusinessNameModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            _logger.LogInformation("Business Name OnGet");
            TradePartyId = id;

            await GetNameAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Business Name OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            TradePartyDTO tradeParty = new()
            {
                Id = TradePartyId,
                PartyName = Name
            };
            await _traderService.UpdateTradePartyAsync(tradeParty);

            return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = TradePartyId });
        }

        private async Task GetNameAsync()
        {
            TradePartyDTO? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null)
                Name = tradeParty.PartyName;
        }
    }
}
