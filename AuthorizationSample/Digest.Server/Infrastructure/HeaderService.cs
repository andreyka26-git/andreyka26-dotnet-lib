using Digest.Server.Application;

namespace Digest.Server.Infrastructure;

public class HeaderService
{
    public Dictionary<string, string> ParseDigestHeaderValue(string digestHeaderValue)
    {
        var values = digestHeaderValue.Substring("Digest".Length).Split(",");

        var valuesDict = new Dictionary<string, string>();

        foreach (var val in values)
        {
            var keyValuePair = val.Split("=", 2);

            valuesDict.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim('\"'));
        }

        return valuesDict;
    }

    public string BuildDigestHeaderValue(List<DigestSubItem> items, string scheme = Consts.Scheme)
    {
        scheme = string.IsNullOrEmpty(scheme) ? string.Empty : $"{scheme} ";
        return $"{scheme}{string.Join(", ", items.Select(FormatHeaderComponent))}";
    }

    private string FormatHeaderComponent(DigestSubItem component)
    {
        if (component.Quote)
        {
            return $"{component.Key}=\"{component.Value}\"";
        }

        return $"{component.Key}={component.Value}";
    }
}
