using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList
{
    public class RegistrationTaskListModel : PageModel
    {
        public Guid? RegistrationID { get; set; }
        public void OnGet()
        {
            RegistrationID = Guid.NewGuid();
        }
    }
}
