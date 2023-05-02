using Defra.Trade.ReMoS.AssuranceService.UI.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting;

[ExcludeFromCodeCoverage]
public class RegisteredBusinessCountryModel : BasePageModel
{
    #region ui model variables
    [BindProperty]
    [Required(ErrorMessage = "Enter a country.")]
    public string? Country { get; set; }
    #endregion

    public async Task<IActionResult> OnGetAsync()
    {
        //Country = getCurrentCountry();
        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        if (checkModelState())
        {
            return RedirectToPage();
        }

        return await OnGetAsync();
    }

    public string getCurrentCountry()
    {
        //add if statement here when API built to check if one exists
        return "";
    }
}
