using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

//Remove when start page added
[ExcludeFromCodeCoverage]
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
