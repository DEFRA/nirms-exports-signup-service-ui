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
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 1 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 1 must be 100 characters or less")]
    [Required(ErrorMessage = "Enter address line 1.")]
    public string LineOne { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter address line 2 using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Address line 2 must be 100 characters or less")]
    public string? LineTwo { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a town or city using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Town or city must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a town or city.")]
    public string CityName { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter a real postcode.")]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a post code.")]
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

        TradePartyDTO tradePartyDto = GenerateDTO(CreateAddressDto());

        if (tradePartyDto.Id == Guid.Empty)
        {
            TraderId = await _traderService.CreateTradePartyAsync(tradePartyDto);
        }
        else
        {
            await _traderService.UpdateTradePartyAddressAsync(tradePartyDto);
        }

        return RedirectToPage(
            Routes.Pages.Path.RegistrationTaskListPath, 
            new { id = TraderId });
    }

    private TradeAddressDTO CreateAddressDto()
    {
        TradeAddressDTO DTO = new()
        {
            Id = AddressId,
            LineOne = LineOne,
            LineTwo = LineTwo,
            CityName = CityName,
            PostCode = PostCode,
        };
        return DTO;
    }

    private TradePartyDTO GenerateDTO(TradeAddressDTO addressDTO)
    {
        return new TradePartyDTO()
        {
            Id = TraderId,
            Address = addressDTO
        };
    }

    private async Task GetAddressFromApiAsync()
    {
        TradePartyDTO? tp = await _traderService.GetTradePartyByIdAsync(TraderId);
        if (tp != null && tp.Address != null)
        {
            AddressId = tp.Address.Id;
            LineOne = tp.Address.LineOne ?? string.Empty;
            LineTwo = tp.Address.LineTwo ?? string.Empty;
            CityName = tp.Address.CityName ?? string.Empty;
            PostCode = tp.Address.PostCode ?? string.Empty;
        }
    }
}