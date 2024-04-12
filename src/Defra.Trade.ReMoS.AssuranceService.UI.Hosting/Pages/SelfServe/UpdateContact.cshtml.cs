using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;

[FeatureGate(FeatureFlags.SelfServe)]
public class UpdateContactModel : BasePageModel<UpdateContactModel>
{
    #region UI Model
    [BindProperty]
    public Guid TradePartyId { get; set; }
    [BindProperty]
    public Guid OrgId { get; set; }

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z\s-']*$", ErrorMessage = "Enter a name using only letters, hyphens or apostrophes")]
    [StringLengthMaximum(50, ErrorMessage = "Name must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a name")]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-_.,/()&]*$", ErrorMessage = "Enter a position using only letters, numbers, brackets, full stops, commas, hyphens, underscores, forward slashes or ampersands")]
    [StringLengthMaximum(50, ErrorMessage = "Position must be 50 characters or less")]
    [Required(ErrorMessage = "Enter a position")]
    public string Position { get; set; } = string.Empty;

    [BindProperty]
    [StringLengthMaximum(100, ErrorMessage = "The email address cannot be longer than 100 characters")]
    [Required(ErrorMessage = "Enter an email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    // This regex pattern supports various formats of UK phone numbers, including landlines and mobile numbers. It allows for optional spaces in different positions.
    [RegularExpression(
        @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?(\d{4}|\d{3}))|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$",
        ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]
    [Required(ErrorMessage = "Enter a telephone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime LastModifiedOn { get; set; }
    public DateTime SubmittedDate { get; set; }
    #endregion UI Model

    public UpdateContactModel(
        ILogger<UpdateContactModel> logger,
        IUserService userService,
        ITraderService traderService) : base(logger, userService, traderService)
    {}

    public async Task<IActionResult> OnGetAsync(Guid Id)
    {
        _logger.LogInformation("Entered {Class}.{Method}", nameof(UpdateContactModel), nameof(OnGetAsync));

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
        _logger.LogInformation("Entered {Class}.{Method}", nameof(UpdateContactModel), nameof(OnPostSubmitAsync));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(OrgId);
        }

        await SubmitContactInfo();
        return RedirectToPage(
            Routes.Pages.Path.SelfServeDashboardPath,
            new { id = OrgId });
    }
    private async Task GetTradePartyInfoFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TradePartyId);
        if (tradeParty != null && tradeParty.Contact != null)
        {
            Name = tradeParty.Contact.PersonName ?? string.Empty;
            PhoneNumber = tradeParty.Contact.TelephoneNumber ?? string.Empty;
            Position = tradeParty.Contact.Position ?? string.Empty;
            Email = tradeParty.Contact.Email ?? string.Empty;
            LastModifiedOn = tradeParty.Contact.LastModifiedDate;
            SubmittedDate = tradeParty.Contact.SubmittedDate;
        }
    }

    private TradePartyDto GenerateDTO()
    {
        return new TradePartyDto()
        {
            Id = TradePartyId,
            ApprovalStatus = Core.Enums.TradePartyApprovalStatus.Approved,
            SignUpRequestSubmittedBy = _userService.GetUserContactId(User),
            Contact = new TradeContactDto()
            {
                PersonName = Name,
                Position = Position,
                Email = Email,
                TelephoneNumber = PhoneNumber,
                LastModifiedDate = DateTime.UtcNow,
                ModifiedBy = _userService.GetUserContactId(User)
            }
        };
    }

    private async Task SubmitContactInfo()
    {
        TradePartyDto tradeParty = GenerateDTO();
        TradePartyId = await _traderService.UpdateTradePartyContactSelfServeAsync(tradeParty);
    }
}
