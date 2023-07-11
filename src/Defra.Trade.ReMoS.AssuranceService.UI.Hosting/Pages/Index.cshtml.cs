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
    private readonly IOptions<EhcoIntegrationSettings> _ehcoIntegrationSettings;

    public IndexModel(ILogger<IndexModel> logger, IOptions<EhcoIntegrationSettings> ehcoIntegrationSettings)
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
                throw new Exception("Model is invalid");
            }

            var token = Request.Form["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage("/Error");
            }

            var decodedToken = DecodeJwt(token);

            var claims = ((JwtSecurityToken)decodedToken).Claims.ToList();            

            var userEnrolledOrganisationsClaims = Request.Form["userEnrolledOrganisationsJWT"];

            if (string.IsNullOrWhiteSpace(userEnrolledOrganisationsClaims))
            {
                return RedirectToPage("/Error");
            }

            claims?.AddRange(userEnrolledOrganisationsClaims.ToString().GetClaims());

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(3), // TODO change to 300
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
            return RedirectToPage("/Error");
        }
    }

    public SecurityToken DecodeJwt(string token)
    {
        var pubKey = "-----BEGIN RSA PUBLIC KEY-----\r\nMIIBCgKCAQEArymNG/U2so2NtU6ledOoO1Rff5gfHam2prsA+iV7NXgUfMOuMH/I\r\nwunTiPz/ZAmmPIwWzaIaqv2b093IH/PDG8AnrFZr75CXVo/Q4XSPdrTHSOIarGNz\r\nZvPBROlnMZQNu+sCzHOieYYX55SHx3mYh5tAivmxXnr37J3ZtGPVES1DemhWpdbG\r\nsQcJMbS90ElAgm+4YFOCrUlIkgDJptDR3YJ+c2mX4F6iLfctmeTzmoruYzyGeRz4\r\nEZ4Ak3Pf6XSJERpO7JDx6GKOlHr/F6SMQjb9SsSuaDM6GptjcFPROwoSN6wCbqr9\r\napC8K+1RzQ4sioxmeV/GAdxnANgajcsdXQIDAQAB\r\n-----END RSA PUBLIC KEY-----";
        var rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportFromPem(pubKey.ToCharArray());

        var secHandler = new JwtSecurityTokenHandler();

        secHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuer = true,    
            ValidAudience = "6c496a6d-d460-40b7-8878-7972b2e53542",
            ValidIssuer = "https://exports-authentication-exp-14995.azurewebsites.net",
            IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
        }, out var decodedToken);

        return decodedToken;
    }
}
