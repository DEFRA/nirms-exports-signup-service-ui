using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Microsoft.Extensions.Primitives;

#pragma warning disable CS1998

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

//Remove when start page added
[ExcludeFromCodeCoverage]
[AllowAnonymous]
[IgnoreAntiforgeryToken(Order = 1001)]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IOptions<EhcoIntegration> _ehcoIntegrationSettings;
    private readonly IValidationParameters _validationParameters;

    public IndexModel(ILogger<IndexModel> logger, IOptions<EhcoIntegration> ehcoIntegrationSettings, IValidationParameters validationParameters)
    {
        _logger = logger;
        _ehcoIntegrationSettings = ehcoIntegrationSettings;
        _validationParameters = validationParameters;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            var correlationId = Guid.NewGuid().ToString();

            var redirect = _ehcoIntegrationSettings.Value.ValidIssuer + "/b2c/remos_signup/login-or-refresh?correlationId=" + correlationId;

            Response.Redirect(redirect);
        }
        else
        {
            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessBusinessPickerPath);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/AuthorizationError");
            }

            var token = Request.Form["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage("/AuthorizationError");
            }

            var decodedToken = DecodeJwt(token, _validationParameters.TokenValidationParameters);

            var claims = ((JwtSecurityToken)decodedToken).Claims.ToList();

            var userEnrolledOrganisationsClaims = Request.Form["userEnrolledOrganisationsJWT"];

            var userEnrolledOrganisationsClaimsList = userEnrolledOrganisationsClaims.ToString().GetClaims();
            claims?.AddRange(userEnrolledOrganisationsClaimsList);

            var IsValid = ValidatePrincipal(claims!);
            if (!IsValid) 
            {
                RedirectToPage("/AuthorizationError");
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(900),
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

            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessBusinessPickerPath);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), ex);
            // Something failed. Redisplay the form.
            return RedirectToPage("/AuthorizationError");
        }
    }

    public SecurityToken DecodeJwt(string token, TokenValidationParameters tokenValidationParameters)
    {       
        var jwtHandler = new JwtSecurityTokenHandler();

        jwtHandler.ValidateToken(token, tokenValidationParameters, out var decodedToken);

        return decodedToken;
    }

    public bool ValidatePrincipal(List<Claim> claims)
    {
        var contactId = claims.Find(c => c.Type == "contactId")!.Value;
        var enrolledOrganisationsCount = claims.Find(c => c.Type == "enrolledOrganisationsCount")!.Value;
        var validAudience = claims.Find(c => c.Type == "aud")!.Value;
        var userEnrolledOrganisations = claims.Find(c => c.Type == "userEnrolledOrganisations")!.Value;

        if (string.IsNullOrWhiteSpace(contactId)
            || string.IsNullOrWhiteSpace(enrolledOrganisationsCount)
            || enrolledOrganisationsCount == "0"
            || validAudience != _validationParameters.TokenValidationParameters.ValidAudience
            || string.IsNullOrEmpty(userEnrolledOrganisations))
        {
            return false;
        }

        return true;
    }

}
