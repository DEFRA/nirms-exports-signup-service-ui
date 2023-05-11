using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    public class RegisteredBusinessNameModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Enter your business name")]
        [RegularExpression(@"^[a-zA-Z0-9\s-_./()&]*$", ErrorMessage = "Enter your business name using only letters, numbers, and special characters -_./()&")]
        [MaxLength(100, ErrorMessage = "Business name is too long")]
        public string Name { get; set; } = string.Empty;

        private readonly ILogger<RegisteredBusinessNameModel> _logger;
        private readonly IGraphqlConsumer _data;

        public RegisteredBusinessNameModel(ILogger<RegisteredBusinessNameModel> logger, IGraphqlConsumer data)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _data = data;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogInformation("Business Name OnGet");
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Business Name OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            var tradeParty = new TradePartyForCreation();
            tradeParty.Name = Name;

            await _data.RegisterTradePartyAsync(tradeParty);

            return Redirect(Routes.RegisteredBusinessType);

        }
    }
}
