using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments
{
    public class PostcodeSearchModel : PageModel
    {
        #region UI Models
        [BindProperty]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter a real postcode.")]
        [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
        [Required(ErrorMessage = "Enter a post code.")]
        public string? Postcode { get; set; } = string.Empty;

        [BindProperty]
        public Guid TradePartyId { get; set; }
        #endregion

        private readonly ITraderService _traderService;
        private readonly ILogger<PostcodeSearchModel> _logger;

        public PostcodeSearchModel(ITraderService traderService, ILogger<PostcodeSearchModel> logger)
        {
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync(Guid id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogTrace("Establishment postcode search on get");
            TradePartyId = id;

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Establishment manual address OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId);
            }


            return RedirectToPage(Routes.Pages.Path.EstablishmentDeparturePostcodeResultPath, new { id = TradePartyId, postcode = Postcode});
        }
    }
}
