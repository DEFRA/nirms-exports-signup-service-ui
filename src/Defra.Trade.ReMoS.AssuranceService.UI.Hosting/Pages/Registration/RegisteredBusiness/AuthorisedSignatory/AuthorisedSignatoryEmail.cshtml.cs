using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    public class AuthorisedSignatoryEmailModel : PageModel
    {
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [BindProperty]
        [Required(ErrorMessage = "Enter a email.")]
        public string? Email { get; set; }
        [BindProperty]
        public string? BusinessName { get; set; }
        [BindProperty]
        public Guid TraderId { get; set; }
        [BindProperty]
        public Guid SignatoryId { get; set; }

        private readonly ITraderService _traderService;
        private readonly ILogger<AuthorisedSignatoryEmailModel> _logger;

        public AuthorisedSignatoryEmailModel(ITraderService traderService, ILogger<AuthorisedSignatoryEmailModel> logger)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TraderId = id;
            
            var party = await GetSignatoryEmailFromApiAsync();
            BusinessName = party?.PartyName;
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Signatory Email OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TraderId);
            }

            var tradeParty = await GenerateDTO();
            await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TraderId });
        }

        private async Task<TradePartyDTO?> GetSignatoryEmailFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
            if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
            {
                SignatoryId = tradeParty.AuthorisedSignatory.Id;
                Email = string.IsNullOrEmpty(Email) ? tradeParty.AuthorisedSignatory.EmailAddress ?? "" : Email;

                return tradeParty;
            }
            return null;
        }

        private async Task<TradePartyDTO> GenerateDTO()
        {
            var tradeParty = await GetSignatoryEmailFromApiAsync();
            return new TradePartyDTO()
            {
                Id = TraderId,
                AuthorisedSignatory = new AuthorisedSignatoryDTO()
                {
                    Id = SignatoryId,
                    Name = tradeParty?.AuthorisedSignatory?.Name,
                    Position = tradeParty?.AuthorisedSignatory?.Position,
                    EmailAddress = Email
                }
            };
        }
    }
}
