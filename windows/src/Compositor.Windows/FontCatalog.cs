using System.Windows.Media;

namespace Compositor.Windows;

public sealed record FontMatch(string Requested, string? ResolvedFamily, bool IsExact);

public sealed class FontCatalog
{
    private readonly Dictionary<string, string> families;

    public FontCatalog()
    {
        families = Fonts.SystemFontFamilies
            .Select(f => f.Source)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(Normalize, f => f, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<string> Families => families.Values.Order().ToArray();

    public FontMatch Resolve(string requested)
    {
        if (families.TryGetValue(Normalize(requested), out var exact))
            return new(requested, exact, true);

        var familyPart = requested.Split('-', StringSplitOptions.RemoveEmptyEntries)[0];
        if (families.TryGetValue(Normalize(familyPart), out var family))
            return new(requested, family, false);

        return new(requested, null, false);
    }

    private static string Normalize(string value) =>
        new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}
