namespace HelloApi;

public class GreetingService
{
    public bool TryGreet(string? name, out string message)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            message = string.Empty;
            return false;
        }

        message = $"Hello, {name.Trim()}!";
        return true;
    }
}
