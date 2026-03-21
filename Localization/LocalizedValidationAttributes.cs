using System.ComponentModel.DataAnnotations;

namespace PickDriverWeb.Localization;

public sealed class LocalizedRequiredAttribute : RequiredAttribute
{
    private readonly string _message;

    public LocalizedRequiredAttribute(string message)
    {
        _message = message;
    }

    public override string FormatErrorMessage(string name) => AppStrings.Translate(_message);
}

public sealed class LocalizedEmailAddressAttribute : ValidationAttribute
{
    private readonly string _message;
    private readonly EmailAddressAttribute _inner = new();

    public LocalizedEmailAddressAttribute(string message)
    {
        _message = message;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (_inner.IsValid(value))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(AppStrings.Translate(_message));
    }
}

public sealed class LocalizedStringLengthAttribute : StringLengthAttribute
{
    private readonly string _message;

    public LocalizedStringLengthAttribute(int maximumLength, string message)
        : base(maximumLength)
    {
        _message = message;
    }

    public override string FormatErrorMessage(string name) => AppStrings.Format(_message, MinimumLength, MaximumLength);
}

public sealed class LocalizedMinLengthAttribute : MinLengthAttribute
{
    private readonly string _message;

    public LocalizedMinLengthAttribute(int length, string message)
        : base(length)
    {
        _message = message;
    }

    public override string FormatErrorMessage(string name) => AppStrings.Format(_message, Length);
}

public sealed class LocalizedCompareAttribute : CompareAttribute
{
    private readonly string _message;

    public LocalizedCompareAttribute(string otherProperty, string message)
        : base(otherProperty)
    {
        _message = message;
    }

    public override string FormatErrorMessage(string name) => AppStrings.Translate(_message);
}

public sealed class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
{
    private readonly string _message;

    public LocalizedRegularExpressionAttribute(string pattern, string message)
        : base(pattern)
    {
        _message = message;
    }

    public override string FormatErrorMessage(string name) => AppStrings.Translate(_message);
}
