using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList
{
    public class RegistrationTaskListModel : PageModel
    {
        #region ui model variables
        [BindProperty]
        public Guid? RegistrationID { get; set; }
        #endregion

        private readonly ILogger<RegistrationTaskListModel> _logger;

        public RegistrationTaskListModel(ILogger<RegistrationTaskListModel> logger)
        {
            _logger = logger;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> OnGetAsync(Guid Id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _logger.LogInformation("OnGet");

            RegistrationID = Id;
            return Page();
        }
    }
}
