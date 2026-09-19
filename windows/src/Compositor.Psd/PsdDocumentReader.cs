using ImageMagick;

namespace Compositor.Psd;

public sealed record ImportedPsdLayer(
    string Name,
    int Width,
    int Height,
    bool IsVisible,
    double Opacity,
    string BlendMode);

public sealed record ImportedPsdDocument(
    PsdHeader Header,
    byte[] CompositePng,
    IReadOnlyList<ImportedPsdLayer> Layers);

public static class PsdDocumentReader
{
    public static ImportedPsdDocument Read(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        using var headerStream = File.OpenRead(path);
        var header = PsdHeaderReader.Read(headerStream);

        using var composite = new MagickImage(path);
        composite.Format = MagickFormat.Png32;
        var preview = composite.ToByteArray();

        using var frames = new MagickImageCollection(path);
        var layers = frames.Select((image, index) =>
        {
            var name = image.GetAttribute("label")
                       ?? image.GetAttribute("psd:layer.name")
                       ?? (index == 0 ? "Composite" : $"Layer {index}");
            return new ImportedPsdLayer(
                name,
                checked((int)image.Width),
                checked((int)image.Height),
                true,
                1.0,
                image.Compose.ToString());
        }).ToArray();

        return new ImportedPsdDocument(header, preview, layers);
    }
}
