using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;


public interface IValidationParameters
{
    TokenValidationParameters TokenValidationParameters { get; }
}

[ExcludeFromCodeCoverage]
public class ValidationParameters : IValidationParameters
{
    public ValidationParameters(IOptions<EhcoIntegration> ehcoIntegrationSettings)
    {
        var pubKey = ehcoIntegrationSettings.Value.PublicKey.ToString();
        var rsaPublicKey = RSA.Create();
        rsaPublicKey.ImportFromPem(pubKey);
        TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuer = true,
            ValidAudience = ehcoIntegrationSettings.Value.ValidAudience,
            ValidIssuer = ehcoIntegrationSettings.Value.ValidIssuer,
            IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
        };
    }

    public TokenValidationParameters TokenValidationParameters { get; }
}


