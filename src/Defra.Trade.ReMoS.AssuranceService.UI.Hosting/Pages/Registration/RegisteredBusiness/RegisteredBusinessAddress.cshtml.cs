using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

public class RegisteredBusinessAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter address line 1 using only letters, numbers, parentheses, dots (.), undescores (_), forward slashes (/), hyphens (-) and apostrophes (')")]
    [StringLength(100, ErrorMessage = "Address line 1 must be 100 characters or less")]
    [Required(ErrorMessage = "Enter address line 1")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter address line 2 using only letters, numbers, parentheses, dots (.), undescores (_), forward slashes (/), hyphens (-) and apostrophes (')")]
    [StringLength(100, ErrorMessage = "Address line 2 must be 100 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-&'._/()]*$", ErrorMessage = "Enter a town or city using only letters, numbers, parentheses, dots (.), undescores (_), forward slashes (/), hyphens (-) and apostrophes (')")]
    [MaxLength(100, ErrorMessage = "Town or city must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a town or city")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})", ErrorMessage = "Enter a real postcode")]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a postcode")]
    public string PostCode { get; set; } = string.Empty;

    [BindProperty]
    public Guid AddressId { get; set; }

    [BindProperty]
    public Guid TraderId { get; set; }
    #endregion

    private readonly ILogger<RegisteredBusinessAddressModel> _logger;
    private readonly ITraderService _traderService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    public RegisteredBusinessAddressModel(
        ILogger<RegisteredBusinessAddressModel> logger,
        ITraderService traderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _traderService = traderService;
    }

    public async Task<IActionResult> OnGetAsync(Guid? id = null)
    {
        TraderId = (TraderId != Guid.Empty) ? TraderId : id ?? Guid.Empty;
        _logger.LogInformation("Address OnGet");

        if (!_traderService.ValidateOrgId(User.Claims, TraderId).Result)
        {
            return RedirectToPage("/Errors/AuthorizationError");
        }

        if (TraderId != Guid.Empty)
        {
            await GetAddressFromApiAsync();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        _logger.LogInformation("Address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        await SubmitAddress();
        return RedirectToPage(
            Routes.Pages.Path.RegisteredBusinessContactNamePath, 
            new { id = TraderId });
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        _logger.LogInformation("Address OnPostSubmit");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync();
        }

        await SubmitAddress();
        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = TraderId });
    }

    #region private methods
    private async Task SubmitAddress()
    {
        TradePartyDto tradePartyDto = GenerateDTO(CreateAddressDto());

        await _traderService.UpdateTradePartyAddressAsync(tradePartyDto);
    }

    private TradeAddressDto CreateAddressDto()
    {
        TradeAddressDto DTO = new()
        {
            Id = AddressId,
            LineOne = LineOne,
            LineTwo = LineTwo,
            CityName = CityName,
            PostCode = PostCode,
        };
        return DTO;
    }

    private TradePartyDto GenerateDTO(TradeAddressDto addressDTO)
    {
        return new TradePartyDto()
        {
            Id = TraderId,
            Address = addressDTO
        };
    }

    private async Task GetAddressFromApiAsync()
    {
        TradePartyDto? tradeParty = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tradeParty != null && tradeParty.Address != null)
        {
            AddressId = tradeParty.Address.Id;
            LineOne = tradeParty.Address.LineOne ?? string.Empty;
            LineTwo = tradeParty.Address.LineTwo ?? string.Empty;
            CityName = tradeParty.Address.CityName ?? string.Empty;
            PostCode = tradeParty.Address.PostCode ?? string.Empty;
        }
    }
    #endregion
}