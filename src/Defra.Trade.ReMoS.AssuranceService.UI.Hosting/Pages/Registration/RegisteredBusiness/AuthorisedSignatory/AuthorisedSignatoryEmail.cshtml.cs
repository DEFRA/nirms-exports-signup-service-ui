using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Management.Sql.Fluent.Models;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8602, CS8601

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    public class AuthorisedSignatoryEmailModel : PageModel
    {
        #region ui model
        [RegularExpression(@"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [BindProperty]
        [Required(ErrorMessage = "Enter an email address")]
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
        private readonly IEstablishmentService _establishmentService;
        private readonly ILogger<AuthorisedSignatoryEmailModel> _logger;

        public AuthorisedSignatoryEmailModel(
            ITraderService traderService, 
            IEstablishmentService establishmentService,
            ILogger<AuthorisedSignatoryEmailModel> logger)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TraderId = id;

            if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var party = await GetSignatoryEmailFromApiAsync();
            BusinessName = party?.PracticeName;
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

            var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TraderId);

            if ( establishments != null && establishments.Any())
            {
                return RedirectToPage(
                    Routes.Pages.Path.AdditionalEstablishmentAddressPath,
                    new { id = TraderId, NI_GBFlag = countryFlag });
            }

            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeSearchPath,
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

        private async Task<TradePartyDto?> GetSignatoryEmailFromApiAsync()
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

        private async Task<TradePartyDto> GenerateDTO()
        {
            var tradeParty = await GetSignatoryEmailFromApiAsync();
            return new TradePartyDto()
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
