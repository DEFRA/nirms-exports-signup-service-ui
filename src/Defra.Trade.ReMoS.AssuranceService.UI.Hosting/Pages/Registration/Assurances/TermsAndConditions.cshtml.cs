using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class TermsAndConditions : PageModel
    {
        #region UI Model

        [BindProperty]
        public Guid TraderId { get; set; }

        [BindProperty]
        public bool TandCs { get; set; }

        #endregion UI Model

        private readonly ITraderService _traderService;
        private readonly IUserService _userService;
        private readonly IEstablishmentService _establishmentService;

        public TermsAndConditions(ITraderService traderService, IUserService userService, IEstablishmentService establishmentService)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _establishmentService = establishmentService ?? throw new ArgumentNullException(nameof(establishmentService));
        }

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
                ModelState.AddModelError(nameof(TandCs), "Confirm that the authorised representative has read and understood the terms and conditions of the scheme");

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

            if (!IsRequiredDataPresent(dto, logisticsLocations!))
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

        private static bool IsRequiredDataPresent(TradePartyDto? dto, IEnumerable<LogisticsLocationDto> logisticsLocations)
        {
            if (dto == null || logisticsLocations == null || !logisticsLocations.Any())
            {
                return false;
            }

            return true;
        }
    }
}