using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Footer;

public class CookiesModel : PageModel
{
    #region prop and ctor

    [BindProperty]
    [Required(ErrorMessage = "Select if you want to accept Google Analytics cookies")]
    public string? Analytics { get; set; } = string.Empty;

    public CookiesModel() 
    { 
    
    }

    #endregion

    public IActionResult OnGet()
    {
        Analytics = Request.Cookies["cookie_policy"];
        return Page();
    }

    public IActionResult OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return OnGet();
        }

        Response.Cookies.Delete("seen_cookie_message");
        Response.Cookies.Delete("cookie_policy");
        Response.Cookies.Append("seen_cookie_message", "yes", new CookieOptions { Expires = DateTime.UtcNow.AddMonths(12), IsEssential = true });

        if (Analytics == "reject")
        {
            Response.Cookies.Append("cookie_policy", "reject", new CookieOptions { Expires = DateTime.UtcNow.AddMonths(12), IsEssential = true });
            Response.Cookies.Delete("_ga_JHVKVL9M7R");
            Response.Cookies.Delete("_ga");
        }
        else
        {
            Response.Cookies.Append("cookie_policy", "accept", new CookieOptions { Expires = DateTime.UtcNow.AddMonths(12), IsEssential = true });
        }

        return RedirectToPage(Routes.Pages.Path.CookiesPath);
    }
}
