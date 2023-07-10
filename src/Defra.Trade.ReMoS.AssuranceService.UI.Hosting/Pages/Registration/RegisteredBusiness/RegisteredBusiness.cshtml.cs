using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    public class RegisteredBusinessModel : PageModel
    {
        [BindProperty]
        public Guid? Id { get; set; }

        public IActionResult OnPostSubmit()
        {
            if (Id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(Id), "Enter Guid");
            }

            return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = Id });
        }

        public IActionResult OnPostSave()
        {
            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessCountryPath, new { id = Guid.Empty });
        }
    }
}
