using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class TermsAndConditions : BasePageModel<TermsAndConditions>
    {
        #region UI Model

        [BindProperty]
        public Guid TraderId { get; set; }

        [BindProperty]
        public bool TandCs { get; set; }

        #endregion UI Model
               
        public TermsAndConditions(
            ITraderService traderService, 
            IUserService userService, 
            IEstablishmentService establishmentService, 
            ICheckAnswersService checkAnswersService) : base(traderService, establishmentService, checkAnswersService, userService)
        {}

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TraderId = id;

            if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TraderId);

            if (dto != null)
            {
                var partyWithSignUpStatus = await _traderService.GetDefraOrgBusinessSignupStatus(dto.OrgId);

                if (partyWithSignUpStatus.signupStatus == Core.Enums.TradePartySignupStatus.Complete)
                {
                    return RedirectToPage(
                        Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                        new { Id = TraderId });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!TandCs)
                ModelState.AddModelError(nameof(TandCs), "Confirm that the authorised representative has read and understood the terms and conditions");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TraderId);
            }

            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TraderId);

            if (dto == null)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = TraderId });
            }

            var logisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(dto.Id);

            if (!_checkAnswersService.IsLogisticsLocationsDataPresent(dto, logisticsLocations!) || !_checkAnswersService.ReadyForCheckAnswers(dto))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = TraderId });
            }

            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;
            dto.SignUpRequestSubmittedBy = _userService.GetUserContactId(User);

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = TraderId });
        }
    }
}