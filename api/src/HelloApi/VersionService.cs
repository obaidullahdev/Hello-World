namespace HelloApi;

public class VersionService
{
    public const string Version = "0.1.0";

    public virtual string GetVersion() => Version;
}
