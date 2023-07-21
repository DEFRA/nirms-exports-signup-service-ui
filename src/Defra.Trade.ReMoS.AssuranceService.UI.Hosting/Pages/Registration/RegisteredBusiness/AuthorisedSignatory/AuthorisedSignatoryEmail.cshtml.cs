using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8602, CS8601

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    public class AuthorisedSignatoryEmailModel : PageModel
    {
        #region ui model
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [BindProperty]
        [Required(ErrorMessage = "Enter the email address of the authorised signatory.")]
        public string? Email { get; set; }
        [BindProperty]
        public string? BusinessName { get; set; }
        [BindProperty]
        public Guid TraderId { get; set; }
        [BindProperty]
        public Guid SignatoryId { get; set; }
        [BindProperty]
        public string? Country { get; set; }
        #endregion

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

            await SubmitEmail();

            string countryFlag = "GB";

            if (Country == "NI")
            {
                countryFlag = "NI";
            }

            return RedirectToPage(
                Routes.Pages.Path.EstablishmentNameAndAddressPath,
                new { id = TraderId, NI_GBFlag = countryFlag });
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            _logger.LogInformation("Signatory Email OnPostSave");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TraderId);
            }

            await SubmitEmail();
            return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = TraderId });
        }

        private async Task SubmitEmail()
        {
            var tradeParty = await GenerateDTO();
            await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);
        }

        private async Task<TradePartyDTO?> GetSignatoryEmailFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
            if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
            {
                SignatoryId = tradeParty.AuthorisedSignatory.Id;
                Email = string.IsNullOrEmpty(Email) ? tradeParty.AuthorisedSignatory.EmailAddress ?? "" : Email;
                Country = tradeParty.Address.TradeCountry;

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
                AuthorisedSignatory = new AuthorisedSignatoryDto()
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
