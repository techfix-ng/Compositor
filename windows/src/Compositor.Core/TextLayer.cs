namespace Compositor.Core;
public enum TextOrientation { Horizontal, Vertical }
public enum ParagraphAlignment { Left, Center, Right, Justify }
public sealed record FontDescriptor(string Family, string? PostScriptName = null, string Style = "Regular", int Weight = 400, int Stretch = 5);
public sealed class TextRun
{
    public required string Text { get; set; }
    public required FontDescriptor Font { get; set; }
    public double Size { get; set; } = 24;
    public double Tracking { get; set; }
    public double Kerning { get; set; }
    public double BaselineShift { get; set; }
    public string Color { get; set; } = "#FFFFFFFF";
}
public sealed class EditableTextLayer
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public IList<TextRun> Runs { get; } = new List<TextRun>();
    public TextOrientation Orientation { get; set; }
    public ParagraphAlignment Alignment { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Rotation { get; set; }
    public double Opacity { get; set; } = 1;
    public bool IsVisible { get; set; } = true;
    public string? OriginalPsdFontName { get; set; }
    public bool HasMissingFont { get; set; }
}
