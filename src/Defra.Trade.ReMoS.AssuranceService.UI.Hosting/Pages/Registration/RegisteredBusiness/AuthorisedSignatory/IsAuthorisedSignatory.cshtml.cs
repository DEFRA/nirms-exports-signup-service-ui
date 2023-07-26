using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
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
        private readonly IEstablishmentService _establishmentService;
        private readonly ILogger<IsAuthorisedSignatoryModel> _logger;

        public IsAuthorisedSignatoryModel(ITraderService traderService, IEstablishmentService establishmentService, ILogger<IsAuthorisedSignatoryModel> logger)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
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

            await SubmitAuthSignatory();

            if (Convert.ToBoolean(IsAuthorisedSignatory))
            {
                var party = await _traderService.GetTradePartyByIdAsync(TradePartyId);
                var establishments = await _establishmentService.GetEstablishmentsForTradePartyAsync(TradePartyId);
                string countryFlag = "GB";

                if (party?.Address?.TradeCountry == "NI")
                {
                    countryFlag = "NI";
                }

                
                if (establishments != null && establishments.Any())
                {
                    return RedirectToPage(
                        Routes.Pages.Path.AdditionalEstablishmentAddressPath,
                        new { id = TradePartyId, NI_GBFlag = countryFlag });
                }

                return RedirectToPage(
                    Routes.Pages.Path.EstablishmentNameAndAddressPath,
                    new { id = TradePartyId, NI_GBFlag = countryFlag });
            }

            return RedirectToPage(Routes.Pages.Path.AuthorisedSignatoryNamePath, new { id = TradePartyId });
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            _logger.LogInformation("IsAuthorisedSignatory OnPostSave");
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }

            await SubmitAuthSignatory();
            return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = TradePartyId });
        }

        #region private methods
        private async Task SubmitAuthSignatory()
        {
            TradePartyDTO tradeParty = await GenerateDTO();
            await _traderService.UpdateTradePartyAsync(tradeParty);

            var updatedTradeParty = await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

            IsAuthorisedSignatory ??= updatedTradeParty?.Contact?.IsAuthorisedSignatory.ToString();

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

            if (tradeParty != null)
            {
                if (isSignatory)
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
                        AuthorisedSignatory = new AuthorisedSignatoryDto()
                        {
                            Id = SignatoryId,
                            Name = tradeParty?.Contact?.PersonName,
                            EmailAddress = tradeParty?.Contact?.Email,
                            Position = tradeParty?.Contact?.Position,
                            TradePartyId = TradePartyId
                        }
                    };
                }

                else
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
                        AuthorisedSignatory = new AuthorisedSignatoryDto()
                        {
                            Id = SignatoryId,
                            Name = null,
                            EmailAddress = null,
                            Position = null,
                            TradePartyId = TradePartyId
                        }
                    };
                }
            }

            return new TradePartyDTO()
            {
                Id = TradePartyId,
                Contact = new TradeContactDTO()
                {
                    Id = ContactId,
                    IsAuthorisedSignatory = isSignatory
                },
                AuthorisedSignatory = new AuthorisedSignatoryDto()
                {
                    Id = SignatoryId,
                    Name = null,
                    EmailAddress = null,
                    Position = null
                }

            };
        }
        #endregion
    }
}