using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Management.BatchAI.Fluent.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
#pragma warning disable CS1998

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

//Remove when start page added
[ExcludeFromCodeCoverage]
[AllowAnonymous]
[IgnoreAntiforgeryToken(Order = 1001)]
public class IndexModel : PageModel
{
    [BindProperty]
    public Guid? Id { get; set; }
    public string? ReturnUrl { get; set; }

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(bool auth = true)
    {
        // check if token valid

        // if valid go to business page (or other)

        // if not valid redirect
        if (auth)
        {
            await Task.Run(() => { });

            var correlationId = Guid.NewGuid().ToString();

            var redirect = $"http://exports-authentication-exp-14943.azurewebsites.net/b2c/remos_signup/login-or-refresh?correlationId={correlationId}";

            Response.Redirect(redirect);
        }




        return Page();
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

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (ModelState.IsValid)
        {
            var token = Request.Form["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Empty Token");
            }

            var claims = token.ToString().GetClaims().ToList();

            var userEnrolledOrganisationsClaims = Request.Form["userEnrolledOrganisationsJWT"];

            if (string.IsNullOrWhiteSpace(userEnrolledOrganisationsClaims))
            {
                throw new Exception("Empty User Enrolled Organisations Token");
            }

            claims.AddRange(userEnrolledOrganisationsClaims.ToString().GetClaims());

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(3000),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {Email} logged in at {Time}.", "user.Email", DateTime.UtcNow);

            return LocalRedirect(Url.GetLocalUrl(returnUrl));
            //return await OnGet(false);
        }

        // Something failed. Redisplay the form.
        return Page();
    }
}
