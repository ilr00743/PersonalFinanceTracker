using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class NotAllowedFutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime <= DateTime.Now;
        }
        
        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return "Transaction date can't be in the future.";
    }
}