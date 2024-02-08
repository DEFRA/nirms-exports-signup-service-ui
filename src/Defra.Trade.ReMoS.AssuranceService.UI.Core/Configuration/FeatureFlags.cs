using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;

[ExcludeFromCodeCoverage]
public static class FeatureFlags
{
    public const string SignUpApplication = "Nirms-SuS-SusToIdcomsSync";
    public const string SelfServe = "Nirms-SuS-SelfServe";
    public const string SelfServeMvpPlus = "Nirms-SuS-SelfServeMvpPlus";
}
