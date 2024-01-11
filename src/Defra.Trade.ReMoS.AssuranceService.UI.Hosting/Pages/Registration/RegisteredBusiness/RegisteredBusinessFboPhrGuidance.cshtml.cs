using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Abstractions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;

[ExcludeFromCodeCoverage]
public class RegisteredBusinessCanNotRegisterModel : BasePageModel<RegisteredBusinessCanNotRegisterModel>
{
    [BindProperty]
    public Guid OrgId { get; set; }

    public IActionResult OnGet(Guid id)
    {
        OrgId = id;
        
        return Page();
    }

    public IActionResult OnPostSubmit()
    {
        return RedirectToPage(
                Routes.Pages.Path.RegisteredBusinessContactNamePath,
                new { id = OrgId });
    }

    public IActionResult OnPostReturnToDashboard()
    {
        return RedirectToPage(
                Routes.Pages.Path.RegistrationTaskListPath,
                new { id = OrgId });
    }
}
