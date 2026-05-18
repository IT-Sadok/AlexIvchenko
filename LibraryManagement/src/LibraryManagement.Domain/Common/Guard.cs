namespace LibraryManagement.Domain.Common;

public static class Guard
{
    public static string RequiredText(
        string? value,
        string parameterName,
        string fieldName,
        int? minimumLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.", parameterName);
        }

        string trimmedValue = value.Trim();

        if (minimumLength.HasValue && trimmedValue.Length < minimumLength.Value)
        {
            throw new ArgumentException(
                $"{fieldName} must contain at least {minimumLength.Value} characters.",
                parameterName);
        }

        return trimmedValue;
    }

    public static int YearNotInFuture(int year, string parameterName, string fieldName)
    {
        int currentYear = DateTime.UtcNow.Year;

        if (year <= 0)
        {
            throw new ArgumentException($"{fieldName} must be greater than 0.", parameterName);
        }

        if (year > currentYear)
        {
            throw new ArgumentException($"{fieldName} cannot be greater than {currentYear}.", parameterName);
        }

        return year;
    }
}