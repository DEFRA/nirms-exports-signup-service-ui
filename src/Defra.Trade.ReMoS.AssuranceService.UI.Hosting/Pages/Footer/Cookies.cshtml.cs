﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Microsoft.Extensions.Options;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Footer;

[ExcludeFromCodeCoverage]
public class CookiesModel : PageModel
{
    #region prop and ctor

    [BindProperty]
    [Required(ErrorMessage = "Select if you want to accept Google Analytics cookies")]
    public string? Analytics { get; set; } = string.Empty;
    [BindProperty]
    public string? MeasurementId { get; set; } = string.Empty;

    private readonly ILogger<CookiesModel> _logger;
    private readonly IOptions<GoogleTagManager> _googleTagManager;

    public CookiesModel(ILogger<CookiesModel> logger, IOptions<GoogleTagManager> googleTagManager) 
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _googleTagManager = googleTagManager ?? throw new ArgumentNullException(nameof(googleTagManager));
    }

    #endregion

    public IActionResult OnGet()
    {
        Analytics = Request.Cookies["cookie_policy"];
        MeasurementId = "_ga_" + _googleTagManager.Value.MeasurementId;
        return Page();
    }

    public IActionResult OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return OnGet();
        }

        _logger.LogInformation("OnPostAsync cookies");

        var cookieOptions = new CookieOptions() 
        { 
            Expires = DateTime.UtcNow.AddMonths(12), 
            IsEssential = true, 
            Secure = true
        };
        Response.Cookies.Delete("seen_cookie_message");
        Response.Cookies.Delete("cookie_policy");
        Response.Cookies.Append("seen_cookie_message", "yes", cookieOptions);

        if (Analytics == "reject")
        {
            Response.Cookies.Append("cookie_policy", "reject", cookieOptions);
            Response.Cookies.Delete(MeasurementId!, new CookieOptions { Path = "/", Domain = _googleTagManager.Value.Domain, Secure = true });
            Response.Cookies.Delete("_ga", new CookieOptions { Path = "/", Domain = _googleTagManager.Value.Domain, Secure = true });
        }
        else
        {
            Response.Cookies.Append("cookie_policy", "accept", cookieOptions);
        }

        return RedirectToPage(Routes.Pages.Path.CookiesPath);
    }
}
