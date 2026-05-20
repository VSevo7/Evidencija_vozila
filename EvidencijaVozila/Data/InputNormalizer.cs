namespace EvidencijaVozila.Data;

public static class InputNormalizer
{
    public static string NormalizeRequired(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    public static string NormalizeRegistrationLookup(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value
            .Trim()
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToUpperInvariant();
    }

    public static string NormalizeRegistrationForStorage(string value)
    {
        return NormalizeRequired(value).ToUpperInvariant();
    }

    public static string NormalizeOrderNumber(string value)
    {
        return NormalizeRequired(value).ToUpperInvariant();
    }

    public static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
