namespace Compositor.Core;

public enum LayerKind { Raster, Group, Text, Adjustment, SmartObject }
public sealed record CanvasSize(int Width, int Height);
public sealed class Layer
{
    public required string Name { get; init; }
    public LayerKind Kind { get; init; }
    public bool IsVisible { get; set; } = true;
    public double Opacity { get; set; } = 1;
    public string BlendMode { get; set; } = "normal";
    public IList<Layer> Children { get; } = new List<Layer>();
}
public sealed class CompositorDocument
{
    public required string Name { get; init; }
    public required CanvasSize Canvas { get; init; }
    public IList<Layer> Layers { get; } = new List<Layer>();
}
