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
        [Required(ErrorMessage = "Enter a postcode.")]
        [BindProperty]
        public string? Postcode { get; set; } = string.Empty;

        [BindProperty]
        public Guid BusinessId { get; set; }
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
            BusinessId = id;

            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            _logger.LogTrace("Establishment postcode search on post");
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(BusinessId);
            }

            return await OnGetAsync(BusinessId);
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Establishment manual address OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(BusinessId);
            }

            return RedirectToPage(Routes.Pages.Path.AdditionalEstablishmentDepartureAddressPath);
        }
    }
}
