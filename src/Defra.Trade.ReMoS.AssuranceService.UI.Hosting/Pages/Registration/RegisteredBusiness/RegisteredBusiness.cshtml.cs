using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness
{
    public class RegisteredBusinessModel : PageModel
    {
        [BindProperty]
        public Guid? Id { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (Id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(Id), "Enter Guid");
            }

            return RedirectToPage(Routes.Pages.Path.RegistrationTaskListPath, new { id = Id });
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessCountryPath, new { id = Guid.Empty });
        }
    }
}
