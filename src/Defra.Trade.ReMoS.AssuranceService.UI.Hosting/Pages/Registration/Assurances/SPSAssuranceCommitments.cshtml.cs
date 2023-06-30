using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class SpsAssuranceCommitmentsModel : PageModel
    {
        #region UI Model
        [BindProperty]
        public Guid TraderId { get; set; }

        [BindProperty]
        public bool AssuranceCommitment { get; set; }
        #endregion

        private readonly ITraderService _traderService;

        public SpsAssuranceCommitmentsModel(ITraderService traderService)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        }

        public IActionResult OnGet(Guid id)
        {
            TraderId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!AssuranceCommitment)
                ModelState.AddModelError(nameof(AssuranceCommitment), "Confirm that the above requirements will be met");

            if (!ModelState.IsValid)
            {
                return OnGet(TraderId);
            }

            TradePartyDTO? dto = await _traderService.GetTradePartyByIdAsync(TraderId);
            dto!.AssuranceCommitmentsSignedDate = DateTime.UtcNow;

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = TraderId });
        }
    }
}
