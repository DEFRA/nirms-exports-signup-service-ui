using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class TermsAndConditions : BasePageModel<TermsAndConditions>
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

        #endregion UI Model

        public TermsAndConditions(
            ITraderService traderService, 
            IUserService userService, 
            IEstablishmentService establishmentService, 
            ICheckAnswersService checkAnswersService,
            ILogger<TermsAndConditions> logger) : base(traderService, establishmentService, checkAnswersService, userService, logger)
        {}

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(TermsAndConditions), nameof(OnGetAsync));

            OrgId = id;
            TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            if (dto != null)
            {
                var partyWithSignUpStatus = await _traderService.GetDefraOrgBusinessSignupStatus(dto.OrgId);

                AuthorisedSignatoryName = dto.AuthorisedSignatory?.Name ?? string.Empty;

                PracticeName = dto.PracticeName ?? string.Empty;
                
                if (partyWithSignUpStatus.signupStatus == TradePartySignupStatus.Complete)
                {
                    return RedirectToPage(
                        Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
                        new { Id = OrgId });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("Entered {Class}.{Method}", nameof(TermsAndConditions), nameof(OnPostSubmitAsync));

            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TradePartyId);

            if (!TandCs)
            {                
                ModelState.AddModelError(nameof(TandCs), $"Confirm that the authorised representative - {dto?.AuthorisedSignatory?.Name} has read and understood the terms and conditions");
            }
                
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(OrgId);
            }            

            if (dto == null)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = OrgId });
            }

            var logisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(dto.Id, false);

            if (!_checkAnswersService.IsLogisticsLocationsDataPresent(dto, logisticsLocations!) || !_checkAnswersService.ReadyForCheckAnswers(dto))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = OrgId });
            }

            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;
            dto.SignUpRequestSubmittedBy = _userService.GetUserContactId(User);
            dto.ApprovalStatus = TradePartyApprovalStatus.PendingApproval;
            dto.Contact!.SubmittedDate = DateTime.UtcNow;
            dto.Contact!.LastModifiedDate = dto.Contact!.SubmittedDate;
            dto.AuthorisedSignatory!.SubmittedDate = DateTime.UtcNow;
            dto.AuthorisedSignatory!.LastModifiedDate = dto.AuthorisedSignatory!.SubmittedDate;

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = OrgId });
        }
    }
}