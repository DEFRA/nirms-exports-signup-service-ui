using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
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
        //[RegularExpression(@"^([Gg][Ii][Rr] 0[Aa]{2}|([A-Za-z][0-9]{1,2}|[A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2}|[A-Za-z][0-9][A-Za-z]|[A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]) ?[0-9][A-Za-z]{2})$", ErrorMessage = "Enter a valid postcode.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Postcode must only contain letters or numbers")]
        [StringLength(100, ErrorMessage = "Postcode must be 100 characters or less")]
        [Required(ErrorMessage = "Enter a postcode.")]
        public string? Postcode { get; set; } = string.Empty;

        public string? ContentHeading { get; set; } = string.Empty;
        public string? ContentText { get; set; } = string.Empty;
        public string? ContextHint { get; set; } = string.Empty;

        [BindProperty]
        public string? NI_GBFlag { get; set; } = string.Empty;

        [BindProperty]
        public Guid TradePartyId { get; set; }
        #endregion

        private readonly ILogger<PostcodeSearchModel> _logger;
        private readonly ITraderService _traderService;

        public PostcodeSearchModel(ILogger<PostcodeSearchModel> logger, ITraderService traderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _traderService = traderService ?? throw new ArgumentNullException(nameof(traderService));
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync(Guid id, string NI_GBFlag = "GB")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogTrace("Establishment postcode search on get");
            TradePartyId = id;
            this.NI_GBFlag = NI_GBFlag;

            if (!_traderService.ValidateOrgId(User.Claims, TradePartyId).Result)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }
            if (_traderService.IsTradePartySignedUp(TradePartyId).Result)
            {
                return RedirectToPage("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
            }

            if (NI_GBFlag == "NI")
            {
                ContextHint = "If your place of destination belongs to a different business";
                ContentHeading = "Add a place of destination";
                ContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
            }
            else
            {
                ContextHint = "If your place of dispatch belongs to a different business";
                ContentHeading = "Add a place of dispatch";
                ContentText = "The locations which are part of your business that consignments to Northern Ireland will depart from under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Establishment manual address OnPostSubmit");

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TradePartyId, NI_GBFlag!);
            }

            return RedirectToPage(
                Routes.Pages.Path.EstablishmentPostcodeResultPath, 
                new { id = TradePartyId, postcode = Postcode, NI_GBFlag});
        }
    }
}
