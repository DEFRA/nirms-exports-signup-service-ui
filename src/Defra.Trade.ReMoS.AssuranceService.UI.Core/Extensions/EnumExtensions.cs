using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;

[ExcludeFromCodeCoverage]
public static class EnumExtensions
{
    static public string GetDescription(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        
        if (field == null)
            return enumValue.ToString();
        
        _ = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        
        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }

        return enumValue.ToString();
    }
}
