using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    public class AuthorisedSignatoryNameModel : PageModel
    {
        [BindProperty]
        [RegularExpression(@"^[a-zA-Z0-9\s-_./()&]*$", ErrorMessage = "Name must only include letters, numbers, and special characters -_./()&")]
        [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
        [Required(ErrorMessage = "Enter a name.")]
        public string Name { get; set; } = string.Empty;
        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid SignatoryId { get; set; }

        private readonly ITraderService _traderService;
        private readonly ILogger<AuthorisedSignatoryNameModel> _logger;

        public AuthorisedSignatoryNameModel(ITraderService traderService, ILogger<AuthorisedSignatoryNameModel> logger)
        {
            _traderService = traderService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TradePartyId = id;
            _logger.LogInformation("Name OnGet");

            await GetSignatoryNameFromApiAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Signatory Name OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            var tradeParty = GenerateDTO();
            await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TradePartyId });
        }

        private async Task GetSignatoryNameFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.AuthorisedSignatory!= null)
            {
                Name = tradeParty.AuthorisedSignatory.Name ?? string.Empty;
            }
        }

        private TradePartyDTO GenerateDTO()
        {
            return new TradePartyDTO()
            {
                Id = TradePartyId,
                AuthorisedSignatory = new AuthorisedSignatoryDTO()
                {
                    Id = SignatoryId,
                    Name = Name
                }
            };
        }
    }
}
