using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8765

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;

public class StringLengthMaximumAttribute : ValidationAttribute
{
    public int Maximum { get; set; }

    public StringLengthMaximumAttribute(int max)
    {
        this.Maximum = max;
    }

    public override bool IsValid(object value)
    {
        string strValue = (string)value;
        if (!string.IsNullOrEmpty(strValue))
        {
            int length = strValue.Length;
            return length <= Maximum;
        }
        return true;
    }
}
