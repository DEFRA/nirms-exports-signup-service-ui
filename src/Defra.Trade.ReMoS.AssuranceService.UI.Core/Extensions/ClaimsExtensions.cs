using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

public static class ClaimsExtensions
{
    /// <summary>
    /// Retrieves claims from token
    /// </summary>
    /// <param name="token">token string</param>
    /// <returns>A collection of claims</returns>
    public static IEnumerable<Claim> GetClaims(this string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwtSecurityTokens = handler.ReadToken(token) as JwtSecurityToken;

        return jwtSecurityTokens!.Claims;
    }
}
