using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.AppService.Fluent.Models;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances
{
    public class TermsAndConditions : BasePageModel<TermsAndConditions>
    {
        #region UI Model

        [BindProperty]
        public Guid TraderId { get; set; }

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
            ICheckAnswersService checkAnswersService) : base(traderService, establishmentService, checkAnswersService, userService)
        {}

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

                AuthorisedSignatoryName = dto.AuthorisedSignatory?.Name ?? string.Empty;

                PracticeName = dto.PracticeName ?? string.Empty;
                
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
            TradePartyDto? dto = await _traderService.GetTradePartyByIdAsync(TraderId);

            if (!TandCs)
            {                
                ModelState.AddModelError(nameof(TandCs), $"Confirm that the authorised representative - {dto?.AuthorisedSignatory?.Name} has read and understood the terms and conditions");
            }
                
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(TraderId);
            }            

            if (dto == null)
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = TraderId });
            }

            var logisticsLocations = await _establishmentService.GetEstablishmentsForTradePartyAsync(dto.Id);

            if (!_checkAnswersService.IsLogisticsLocationsDataPresent(dto, logisticsLocations!) || !_checkAnswersService.ReadyForCheckAnswers(dto))
            {
                return RedirectToPage(
                    Routes.Pages.Path.RegistrationTaskListPath,
                        new { id = TraderId });
            }

            dto!.TermsAndConditionsSignedDate = DateTime.UtcNow;
            dto.SignUpRequestSubmittedBy = _userService.GetUserContactId(User);
            dto.Contact!.SubmittedDate = DateTime.UtcNow;
            dto.Contact!.LastModifiedDate = dto.Contact!.SubmittedDate;
            dto.AuthorisedSignatory!.SubmittedDate = DateTime.UtcNow;
            dto.AuthorisedSignatory!.LastModifiedDate = dto.AuthorisedSignatory!.SubmittedDate;

            await _traderService.UpdateTradePartyAsync(dto);

            return RedirectToPage(
                Routes.Pages.Path.SignUpConfirmationPath,
                new { id = TraderId });
        }
    }
}