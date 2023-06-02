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
        [StringLength(100, ErrorMessage = "Postcode must be 100 characters or less")]
        [Required(ErrorMessage = "Enter a postcode.")]
        public string? Postcode { get; set; } = string.Empty;

        public string ContentHeading { get; set; } = string.Empty;
        public string ContentText { get; set; } = string.Empty;

        [BindProperty]
        public string NI_GBFlag { get; set; } = string.Empty;

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
        public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogTrace("Establishment postcode search on get");
            TradePartyId = id;

            if (NI_GBFlag == "NI") 
            {
                ContentHeading = "Add a point of destination (optional)";
                ContentText = "Add all establishments in Northern Ireland where your goods go after the port of entry. For example, a hub or store.";
            }
            else
            {
                ContentHeading = "Add a point of departure";
                ContentText = "Add all establishments in Great Britan from which your goods will be departing under the scheme.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Establishment manual address OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId, NI_GBFlag);
            }

            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeResultPath, 
                new { id = TradePartyId, postcode = Postcode, NI_GBFlag});
        }
    }
}
