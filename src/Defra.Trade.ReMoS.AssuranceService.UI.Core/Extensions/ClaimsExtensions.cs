using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

public static class ClaimsExtensions
{
    public static IEnumerable<Claim> GetClaims(this string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwtSecurityTokens = handler.ReadToken(token) as JwtSecurityToken;

        return jwtSecurityTokens!.Claims;
    }
}
