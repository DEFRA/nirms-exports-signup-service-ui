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

    public IndexModel(ILogger<IndexModel> logger, IOptions<EhcoIntegration> ehcoIntegrationSettings)
    {
        _logger = logger;
        _ehcoIntegrationSettings = ehcoIntegrationSettings;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            await Task.Run(() => { });

            var correlationId = Guid.NewGuid().ToString();

            var redirect = _ehcoIntegrationSettings.Value.EhcoAuthEndpoint + correlationId;

            Response.Redirect(redirect);
        }
        else
        {
            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessPath);
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

            var pubKey = _ehcoIntegrationSettings.Value.PublicKey.ToString();
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(pubKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidAudience = _ehcoIntegrationSettings.Value.ValidAudience,
                ValidIssuer = _ehcoIntegrationSettings.Value.ValidIssuer,
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
            };

            var decodedToken = DecodeJwt(token, validationParameters);

            var claims = ((JwtSecurityToken)decodedToken).Claims.ToList();            

            var userEnrolledOrganisationsClaims = Request.Form["userEnrolledOrganisationsJWT"];

            if (string.IsNullOrWhiteSpace(userEnrolledOrganisationsClaims))
            {
                return RedirectToPage("/AuthorizationError");
            }

            claims?.AddRange(userEnrolledOrganisationsClaims.ToString().GetClaims());

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(3), // TODO change to 900
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

            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessPath);

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
}
