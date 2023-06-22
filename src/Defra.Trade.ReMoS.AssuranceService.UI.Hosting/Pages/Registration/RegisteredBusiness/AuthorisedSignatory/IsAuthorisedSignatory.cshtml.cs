using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.AuthorisedSignatory
{
    public class IsAuthorisedSignatoryModel : PageModel
    {
        [BindProperty]      
        [Required(ErrorMessage = "Fill in Yes or No")]
        public string? IsAuthorisedSignatory { get; set; } = null;
        [BindProperty]
        public string? BusinessName { get; set; }

        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid ContactId { get; set; }
        [BindProperty]
        public Guid SignatoryId { get; set; }

        private readonly ITraderService _traderService;
        private readonly ILogger<IsAuthorisedSignatoryModel> _logger;

        public IsAuthorisedSignatoryModel(ITraderService traderService, ILogger<IsAuthorisedSignatoryModel> logger)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TradePartyId = id;
            _logger.LogInformation("IsAuthorisedSignatory onGet");
            var party = await GetIsAuthorisedSignatoryFromApiAsync();
            BusinessName = party?.PartyName;

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("IsAuthorisedSignatory OnPostSubmit");
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            TradePartyDTO tradeParty = await GenerateDTO();

            var updatedTradeParty = await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);
            IsAuthorisedSignatory = updatedTradeParty?.Contact?.IsAuthorisedSignatory.ToString();
            if (Convert.ToBoolean(IsAuthorisedSignatory))
            {
                return RedirectToPage(Routes.Pages.Path.EstablishmentPostcodeSearchPath, new { id = TradePartyId });
            }

            return RedirectToPage(Routes.Pages.Path.AuthorisedSignatoryNamePath, new { id = TradePartyId });
        }

        private async Task<TradePartyDTO?> GetIsAuthorisedSignatoryFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.Contact != null && tradeParty.AuthorisedSignatory != null)
            {
                IsAuthorisedSignatory ??= tradeParty.Contact.IsAuthorisedSignatory.ToString();
                ContactId = tradeParty.Contact.Id;
                SignatoryId = tradeParty.AuthorisedSignatory.Id;
            }

            return tradeParty;
        }

        private async Task<TradePartyDTO> GenerateDTO()
        {
            var tradeParty = await GetIsAuthorisedSignatoryFromApiAsync();

            var isSignatory = Convert.ToBoolean(IsAuthorisedSignatory);
            if (isSignatory && tradeParty != null)
            {
                return new TradePartyDTO()
                {
                    Id = TradePartyId,
                    Contact = new TradeContactDTO()
                    {
                        Id = ContactId,
                        PersonName = tradeParty?.Contact?.PersonName,
                        Email = tradeParty?.Contact?.Email,
                        Position = tradeParty?.Contact?.Position,
                        TelephoneNumber = tradeParty?.Contact?.TelephoneNumber,
                        IsAuthorisedSignatory = isSignatory
                    },
                    AuthorisedSignatory = new AuthorisedSignatoryDTO()
                    {
                        Id = SignatoryId,
                        Name = tradeParty?.Contact?.PersonName,
                        EmailAddress = tradeParty?.Contact?.Email,
                        Position = tradeParty?.Contact?.Position,
                        TradePartyId = TradePartyId
                    }
                };

            }

            return new TradePartyDTO()
            {
                Id = TradePartyId,
                Contact = new TradeContactDTO()
                {
                    Id = ContactId,
                    IsAuthorisedSignatory = isSignatory
                },
                AuthorisedSignatory = new AuthorisedSignatoryDTO()
                {
                    Id = SignatoryId,
                    Name = null,
                    EmailAddress = null,
                    Position = null
                }

            };
        }
    }
}