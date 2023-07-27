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
        #endregion

        private readonly ITraderService _traderService;

        public TermsAndConditions(ITraderService traderService)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            TraderId = id;

            TradePartyDTO? dto = await _traderService.GetTradePartyByIdAsync(TraderId);

            if (dto != null)
            {
                var partyWithSignUpStatus = await _traderService.GetDefraOrgBusinessSignupStatus(dto.OrgId);

                if (partyWithSignUpStatus.signupStatus == Core.Enums.TradePartySignupStatus.Complete)
                {
                    return RedirectToPage(
                        Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                        new { TraderId = TraderId });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!TandCs)
                ModelState.AddModelError(nameof(TandCs), "Confirm that the authorised representative has read and understood the terms of conditions of the scheme");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TraderId);
            }

            TradePartyDTO? dto = await _traderService.GetTradePartyByIdAsync(TraderId);

            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = TraderId });
        }
    }
}
