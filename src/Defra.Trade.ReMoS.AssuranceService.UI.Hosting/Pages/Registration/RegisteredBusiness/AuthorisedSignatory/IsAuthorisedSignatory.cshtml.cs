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
        [Required(ErrorMessage = "Select if the contact person is the authorised representative")]
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

            if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(id).Result)
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            _logger.LogInformation("IsAuthorisedSignatory onGet");
            var party = await GetIsAuthorisedSignatoryFromApiAsync();
            BusinessName = party?.PracticeName;

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
                    Routes.Pages.Path.EstablishmentPostcodeSearchPath,
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
            TradePartyDto tradeParty = await GenerateDTO();
            await _traderService.UpdateTradePartyAsync(tradeParty);

            var updatedTradeParty = await _traderService.UpdateAuthorisedSignatoryAsync(tradeParty);

            IsAuthorisedSignatory ??= updatedTradeParty?.Contact?.IsAuthorisedSignatory.ToString();

        }

        private async Task<TradePartyDto?> GetIsAuthorisedSignatoryFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            IsAuthorisedSignatory ??= tradeParty?.Contact?.IsAuthorisedSignatory.ToString();
            if (tradeParty != null && tradeParty.Contact != null && tradeParty.AuthorisedSignatory != null)
            {
                ContactId = tradeParty.Contact.Id;
                SignatoryId = tradeParty.AuthorisedSignatory.Id;
            }

            return tradeParty;
        }

        private async Task<AuthorisedSignatoryDto?> GetAuthorisedSignatoryFromApiAsync()
        {
            var tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            return tradeParty?.AuthorisedSignatory;
        }

        private async Task<TradePartyDto> GenerateDTO()
        {
            var tradeParty = await GetIsAuthorisedSignatoryFromApiAsync();

            //Checking if there was previously an authorised signatory assigned
            var previousState = tradeParty?.Contact?.IsAuthorisedSignatory;

            var isSignatory = Convert.ToBoolean(IsAuthorisedSignatory);

            var authorisedSignatoryStub = new AuthorisedSignatoryDto 
            {
                Id = SignatoryId,
            };

            var tradeContactDto = new TradeContactDto
            {
                Id = ContactId,
                PersonName = (tradeParty != null) ? tradeParty.Contact?.PersonName : null,
                Email = (tradeParty != null) ? tradeParty.Contact?.Email : null,
                Position = (tradeParty != null) ? tradeParty.Contact?.Position : null,
                TelephoneNumber = (tradeParty != null) ? tradeParty.Contact?.TelephoneNumber : null,
                IsAuthorisedSignatory = isSignatory
            };

            if (tradeParty != null)
            {
                if (isSignatory)
                {
                    authorisedSignatoryStub.Name = tradeParty.Contact?.PersonName;
                    authorisedSignatoryStub.EmailAddress = tradeParty.Contact?.Email;
                    authorisedSignatoryStub.Position = tradeParty.Contact?.Position;
                    authorisedSignatoryStub.TradePartyId = TradePartyId;

                    return new TradePartyDto()
                    {
                        Id = TradePartyId,
                        Contact = tradeContactDto,
                        AuthorisedSignatory = authorisedSignatoryStub
                    };
                }

                else
                {
                    if (previousState == true)
                    {
                        authorisedSignatoryStub.TradePartyId = TradePartyId;

                        return new TradePartyDto()
                        {
                            Id = TradePartyId,
                            Contact = tradeContactDto,
                            AuthorisedSignatory = authorisedSignatoryStub
                        };
                    }
                    else
                    {
                        return new TradePartyDto()
                        {
                            Id = TradePartyId,
                            Contact = tradeContactDto,
                            AuthorisedSignatory = await GetAuthorisedSignatoryFromApiAsync()
                        };
                    }
                }
            }

            return new TradePartyDto()
            {
                Id = TradePartyId,
                Contact = new TradeContactDto()
                {
                    Id = ContactId,
                    IsAuthorisedSignatory = isSignatory
                },
                AuthorisedSignatory = authorisedSignatoryStub
            };
        }
        #endregion
    }
}