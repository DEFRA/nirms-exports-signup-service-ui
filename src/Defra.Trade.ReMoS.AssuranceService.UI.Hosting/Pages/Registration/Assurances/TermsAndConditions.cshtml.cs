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
        private readonly IUserService _userService;

        public TermsAndConditions(ITraderService traderService, IUserService userService)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IActionResult OnGet(Guid id)
        {
            TraderId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!TandCs)
                ModelState.AddModelError(nameof(TandCs), "Confirm that the authorised representative has read and understood the terms of conditions of the scheme");

            if (!ModelState.IsValid)
            {
                return OnGet(TraderId);
            }

            TradePartyDTO? dto = await _traderService.GetTradePartyByIdAsync(TraderId);
            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;
            dto.SignUpRequestSubmittedBy = _userService.GetUserContactId(User);

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = TraderId });
        }
    }
}
