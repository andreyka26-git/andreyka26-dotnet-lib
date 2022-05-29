namespace Digest.Server.Infrastructure;

public class HeaderService
{
    public string FormatHeaderComponent((string Key, string Value, bool ShouldQuote) component)
    {
        if (component.ShouldQuote)
        {
            return $"{component.Key}=\"{component.Value}\"";
        }

        return $"{component.Key}={component.Value}";
    }
}
