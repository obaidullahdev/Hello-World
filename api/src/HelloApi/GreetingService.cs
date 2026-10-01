namespace HelloApi;

public class GreetingService
{
    public const int MaxNameLength = 50;
    public const string BlankNameError = "Name must not be blank.";
    public const string NameTooLongError = "Name must not be longer than 50 characters.";

    public string? Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BlankNameError;
        }

        return name.Trim().Length > MaxNameLength ? NameTooLongError : null;
    }

    public bool TryGreet(string? name, out string message)
    {
        if (Validate(name) is not null)
        {
            message = string.Empty;
            return false;
        }

        message = $"Hello, {name!.Trim()}!";
        return true;
    }
}
