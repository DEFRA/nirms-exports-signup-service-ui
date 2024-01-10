using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
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
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;
using Microsoft.FeatureManagement;

namespace Defra.ReMoS.AssuranceService.UI.Hosting.Pages;

//Remove when start page added
[ExcludeFromCodeCoverage]
[AllowAnonymous]
[IgnoreAntiforgeryToken(Order = 1001)]
public class IndexModel : PageModel
{
    [BindProperty] public string? Password { get; set; } = default!;
    [BindProperty] public bool UseMagicWord { get; set; } = default!;

    private readonly ILogger<IndexModel> _logger;
    private readonly IOptions<EhcoIntegration> _ehcoIntegrationSettings;
    private readonly IValidationParameters _validationParameters;
    private readonly IConfiguration _configuration;
    private readonly IFeatureManager _featureManager;

    public IndexModel(
        ILogger<IndexModel> logger,
        IOptions<EhcoIntegration> ehcoIntegrationSettings,
        IValidationParameters validationParameters,
        IConfiguration configuration,
        IFeatureManager featureManager)
    {
        _logger = logger;
        _ehcoIntegrationSettings = ehcoIntegrationSettings;
        _validationParameters = validationParameters;
        _configuration = configuration;
        _featureManager = featureManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        //Test Feature Flag
        if (await _featureManager.IsEnabledAsync(FeatureFlags.SelfServe))
        {
            // Run the following code
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                var correlationId = Guid.NewGuid().ToString();
                var redirect = _ehcoIntegrationSettings.Value.ValidIssuer + "/b2c/remos_signup/login-or-refresh?correlationId=" + correlationId;
                Response.Redirect(redirect);
                return Page();
            }
            else
            {
                return RedirectToPage(Routes.Pages.Path.BusinessListPath);
            }
        }

        if (_configuration.GetValue<bool>("ReMoS:MagicWordEnabled"))
        {
            UseMagicWord = true;
        }
        else
        {
            UseMagicWord = false;
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
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        if (_configuration.GetValue<bool>("ReMoS:MagicWordEnabled"))
        {
            if (Password == _configuration.GetValue<string>("ReMoS:MagicWord"))
            {
                if (User.Identity == null || !User.Identity.IsAuthenticated)
                {
                    var correlationId = Guid.NewGuid().ToString();

                    var redirect = _ehcoIntegrationSettings.Value.ValidIssuer + "/b2c/remos_signup/login-or-refresh?correlationId=" + correlationId;

                    Response.Redirect(redirect);
                    return Page();
                }
                else
                {
                    return RedirectToPage(Routes.Pages.Path.RegisteredBusinessBusinessPickerPath);
                }
            }
            else
            {
                ModelState.AddModelError(nameof(Password), "Enter the correct password");
                return await OnGetAsync();
            }
        }
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var token = Request.Form["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage("/Errors/AuthorizationError");
            }

            var decodedToken = DecodeJwt(token, _validationParameters.TokenValidationParameters);

            var claims = ((JwtSecurityToken)decodedToken).Claims.ToList();

            var userEnrolledOrganisationsClaims = Request.Form["userEnrolledOrganisationsJWT"];

            var userEnrolledOrganisationsClaimsList = userEnrolledOrganisationsClaims.ToString().GetClaims();
            claims.AddRange(userEnrolledOrganisationsClaimsList);

            var IsValid = ValidatePrincipal(claims!);
            if (!IsValid)
            {
                RedirectToPage("/Errors/AuthorizationError");
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = UnixSecondsToDateTime(claims!.Find(c => c.Type == "exp")!.Value),
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User {Email} logged in at {Time}.", "user.Email", DateTime.UtcNow);

            if (await _featureManager.IsEnabledAsync(FeatureFlags.SelfServe))
                return RedirectToPage(Routes.Pages.Path.BusinessListPath);

            return RedirectToPage(Routes.Pages.Path.RegisteredBusinessBusinessPickerPath);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), ex);
            // Something failed. Redisplay the form.
            return RedirectToPage("/Errors/AuthorizationError");
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
        var exp = claims.Find(c => c.Type == "exp")!.Value;

        if (string.IsNullOrWhiteSpace(contactId)
            || string.IsNullOrWhiteSpace(enrolledOrganisationsCount)
            || enrolledOrganisationsCount == "0"
            || validAudience != _validationParameters.TokenValidationParameters.ValidAudience
            || string.IsNullOrEmpty(userEnrolledOrganisations)
            || string.IsNullOrWhiteSpace(exp))
        {
            return false;
        }

        return true;
    }

    private static DateTime UnixSecondsToDateTime(string timestamp)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp));
        return offset.UtcDateTime;
    }

}
