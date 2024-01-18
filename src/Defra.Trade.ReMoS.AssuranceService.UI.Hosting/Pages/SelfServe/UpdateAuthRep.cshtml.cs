using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe
{
    public class UpdateAuthRepModel : BasePageModel<UpdateAuthRepModel>
    {
        #region UI Model
        [BindProperty]
        public Guid TradePartyId { get; set; }
        [BindProperty]
        public Guid OrgId { get; set; }

        [BindProperty]
        [RegularExpression(@"^[a-zA-Z\s-']*$", ErrorMessage = "Enter a name using only letters, hyphens or apostrophes")]
        [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
        [Required(ErrorMessage = "Enter a name")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [RegularExpression(
            @"^\w+([-.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
            ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [StringLength(100, ErrorMessage = "The email address cannot be longer than 100 characters")]
        [Required(ErrorMessage = "Enter an email address")]
        public string? Email { get; set; }

        [BindProperty]
        [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
        [StringLength(50, ErrorMessage = "Position must be 50 characters or less")]
        [Required(ErrorMessage = "Enter a position")]
        public string Position { get; set; } = string.Empty;
        [BindProperty]
        public bool TandCs { get; set; } = false;
        public DateTime LastModifiedOn { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string? BusinessName { get; set; }
        #endregion


        public UpdateAuthRepModel(
        ILogger<UpdateAuthRepModel> logger,
        IUserService userService,
        ITraderService traderService) : base(logger, userService, traderService)
        { }
        public async Task<IActionResult> OnGetAsync(Guid Id)
        {
            _logger.LogInformation("Self Serve Update Auth rep OnGet");
            OrgId = Id;
            TradePartyId = _traderService.GetTradePartyByOrgIdAsync(OrgId).Result!.Id;

            if (!_traderService.ValidateOrgId(User.Claims, OrgId))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            await GetTradePartyInfoFromApiAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            _logger.LogInformation("SelfServe Update Auth Rep OnPostSubmit");

            if (!TandCs)
            {
                ModelState.AddModelError(nameof(TandCs), $"Confirm that {Name} has read and understood the terms and conditions");
            }

            if (!ModelState.IsValid)
            {
                return await OnGetAsync(OrgId);
            }

            await SubmitAuthRepInfo();
            return RedirectToPage(
                Routes.Pages.Path.SelfServeDashboardPath,
                new { id = OrgId });
        }

        private async Task GetTradePartyInfoFromApiAsync()
        {
            TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
            if (tradeParty != null && tradeParty.AuthorisedSignatory != null)
            {
                Name = tradeParty.AuthorisedSignatory.Name ?? string.Empty;
                Position = tradeParty.AuthorisedSignatory.Position ?? string.Empty;
                Email = tradeParty.AuthorisedSignatory.EmailAddress ?? string.Empty;
                LastModifiedOn = tradeParty.AuthorisedSignatory.LastModifiedDate;
                SubmittedDate = tradeParty.AuthorisedSignatory.SubmittedDate;
                BusinessName = tradeParty.PracticeName!;
            }
        }

        private TradePartyDto GenerateDTO()
        {
            return new TradePartyDto()
            {
                Id = TradePartyId,
                ApprovalStatus = Core.Enums.TradePartyApprovalStatus.Approved,
                SignUpRequestSubmittedBy = _userService.GetUserContactId(User),
                AuthorisedSignatory = new AuthorisedSignatoryDto()
                {
                    Name = Name,
                    Position = Position,
                    EmailAddress = Email,
                    LastModifiedDate = DateTime.UtcNow,
                    ModifiedBy = _userService.GetUserContactId(User)
                }
            };
        }

        private async Task SubmitAuthRepInfo()
        {
            TradePartyDto tradeParty = GenerateDTO();
            TradePartyId = await _traderService.UpdateAuthRepSelfServeAsync(tradeParty);
        }
    }
}
