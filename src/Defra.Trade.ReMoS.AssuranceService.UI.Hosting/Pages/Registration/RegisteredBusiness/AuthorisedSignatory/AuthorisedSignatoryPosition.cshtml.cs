using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    [ExcludeFromCodeCoverage]
    public class AuthorisedSignatoryPositionModel : PageModel
    {
        [BindProperty]
        [RegularExpression(@"^[a-zA-Z0-9\s-_./()&]*$", ErrorMessage = "Position must only include letters, numbers, and special characters -_./()&")]
        [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
        [Required(ErrorMessage = "Enter a position.")]
        public string Position { get; set; } = string.Empty;
        [BindProperty]
        public string? BusinessName { get; set; }
        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid SignatoryId { get; set; }

        private readonly ITraderService _traderService;
        private readonly ILogger<AuthorisedSignatoryNameModel> _logger;

        public AuthorisedSignatoryPositionModel(ITraderService traderService, ILogger<AuthorisedSignatoryNameModel> logger)
        {
            _traderService = traderService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TradePartyId = id;
            _logger.LogInformation("Position OnGet");

            _ = await GetSignatoryPosFromApiAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Signatory Position OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            var tradeParty = await GenerateDTO();
            await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

            return RedirectToPage(
                Routes.Pages.Path.AuthorisedSignatoryEmailPath,
                new { id = TradePartyId });
        }

        private async Task<TradePartyDTO?> GetSignatoryPosFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
            {
                SignatoryId = tradeParty.AuthorisedSignatory.Id;
                Position = string.IsNullOrEmpty(Position) ? tradeParty.AuthorisedSignatory.Position ?? "" : Position;
                BusinessName = tradeParty.PartyName;

                return tradeParty;
            }

            return null;
        }

        private async Task<TradePartyDTO> GenerateDTO()
        {
            var tradeParty = await GetSignatoryPosFromApiAsync();
            return new TradePartyDTO()
            {
                Id = TradePartyId,
                AuthorisedSignatory = new AuthorisedSignatoryDTO()
                {
                    Id = SignatoryId,
                    Name = tradeParty?.AuthorisedSignatory?.Name,
                    Position = Position,
                    EmailAddress = tradeParty?.AuthorisedSignatory?.EmailAddress
                }
            };
        }
    }
}
