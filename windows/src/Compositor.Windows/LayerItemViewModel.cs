using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Compositor.Windows;
public sealed class LayerItemViewModel : INotifyPropertyChanged
{
    private bool isVisible = true;
    public required string Name { get; init; }
    public string Kind { get; init; } = "Raster";
    public object? CanvasElement { get; init; }
    public bool IsVisible { get => isVisible; set { if (isVisible == value) return; isVisible = value; if (CanvasElement is System.Windows.UIElement e) e.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
