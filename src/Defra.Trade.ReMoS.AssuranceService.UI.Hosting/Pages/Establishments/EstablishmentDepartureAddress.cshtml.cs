using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;

public class EstablishmentDepartureAddressModel : PageModel
{
    #region ui model variables
    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter establishment name using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Establishment name must be 100 characters or less")]
    [Required(ErrorMessage = "Enter establishment name.")]
    public string EstablishmentName { get; set; } = string.Empty;

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
    [RegularExpression(@"^[a-zA-Z0-9\s-']*$", ErrorMessage = "Enter a country using only letters, numbers, hyphens (-) and apostrophes (').")]
    [StringLength(100, ErrorMessage = "Country must be 100 characters or less")]
    public string? Country { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Enter a real postcode.")]
    [StringLength(100, ErrorMessage = "Post code must be 100 characters or less")]
    [Required(ErrorMessage = "Enter a post code.")]
    public string PostCode { get; set; } = string.Empty;

    [BindProperty]
    public Guid EstablishmentId { get; set; }
    #endregion
    public void OnGet()
    {
    }
}
