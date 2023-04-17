using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.ReMoS.AssuranceService.UI.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public string Message { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Message = "Hello World!";
    }

    public void OnPost()
    {
        if (!ModelState.IsValid) 
        {
            ErrorMessage = "There is an error";
        }
    }
}
