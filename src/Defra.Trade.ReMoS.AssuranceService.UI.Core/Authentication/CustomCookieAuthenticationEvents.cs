using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;

        var contactId = userPrincipal!.Claims.FirstOrDefault(c => c.Type == "contactId");

        if (contactId == null)
        {
            await RejectAndSignOut(context);
        }

        var enrolledOrganisationsCount = userPrincipal!.Claims.FirstOrDefault(c => c.Type == "enrolledOrganisationsCount");

        if (enrolledOrganisationsCount == null)
        {
            await RejectAndSignOut(context);
        }
    }

    private async static Task RejectAndSignOut(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
