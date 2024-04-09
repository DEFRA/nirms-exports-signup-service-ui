using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class UpdatedTermsAndConditions : BasePageModel<UpdatedTermsAndConditions>
    {
        #region UI Model

        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid OrgId { get; set; }

        [BindProperty]
        public bool TandCs { get; set; }
        [BindProperty]
        public string? AuthorisedSignatoryName { get; set; } = string.Empty;
        [BindProperty]
        public string? PracticeName { get; set; } = string.Empty;
        [BindProperty]
        public string? UpdatedTermsAndConditionsDate { get; set; } = string.Empty;

        #endregion UI Model
        public UpdatedTermsAndConditions(
            ITraderService traderService,
            IUserService userService,
            IConfiguration config,
            ILogger<UpdatedTermsAndConditions> logger) : base(traderService, userService, config, logger)
        { }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(UpdatedTermsAndConditions), nameof(OnGetAsync));

            OrgId = id;
            TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;
            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            var updatedDate = _config.GetValue<string>("UpdatedTermsAndConditionsDate");
            if ((updatedDate == null) || (dto!.TermsAndConditionsSignedDate >= DateTime.ParseExact(updatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
            {
                return RedirectToPage(Routes.Pages.Path.SelfServeDashboardPath, new { id = OrgId });
            }
            else UpdatedTermsAndConditionsDate = DateTime.ParseExact(updatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("d MMMM yyyy");

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            AuthorisedSignatoryName = dto.AuthorisedSignatory?.Name ?? string.Empty;
            PracticeName = dto.PracticeName ?? string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(UpdatedTermsAndConditions), nameof(OnPostSubmitAsync));

            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            if (!TandCs)
            {
                ModelState.AddModelError(nameof(TandCs), $"Confirm that the authorised representative - {dto?.AuthorisedSignatory?.Name} has read and understood the terms and conditions, or click ‘Skip for now’");
            }

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(OrgId);
            }

            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;
            dto.SignUpRequestSubmittedBy = _userService.GetUserContactId(User);

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SelfServeDashboardPath,
                new { id = OrgId });
        }
    }
}
