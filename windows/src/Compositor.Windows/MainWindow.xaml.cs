using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Compositor.Psd;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Compositor.Windows;
public partial class MainWindow : Window
{
    private double zoom = 1;
    private string activeTool = "Move / Transform";
    private readonly FontCatalog fontCatalog = new();
    private readonly ObservableCollection<LayerItemViewModel> layers = new();
    private TextBox? activeTextBox;
    private Brush activeTextColor = Brushes.White;
    public MainWindow()
    {
        InitializeComponent();
        LayersList.ItemsSource = layers;
        FontFamilyBox.ItemsSource = fontCatalog.Families;
        FontFamilyBox.SelectedItem = fontCatalog.Resolve("Segoe UI").ResolvedFamily ?? fontCatalog.Families.FirstOrDefault();
    }
    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var d = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff;*.psd;*.psb|All files|*.*" };
        if (d.ShowDialog(this) != true) return;
        try {
            var ext = Path.GetExtension(d.FileName).ToLowerInvariant();
            if (ext is ".psd" or ".psb") {
                StatusText.Text = $"Opening {Path.GetFileName(d.FileName)}…";
                var document = PsdDocumentReader.Read(d.FileName);
                using var memory = new MemoryStream(document.CompositePng);
                var preview = new BitmapImage(); preview.BeginInit(); preview.CacheOption=BitmapCacheOption.OnLoad; preview.StreamSource=memory; preview.EndInit(); preview.Freeze();
                CanvasImage.Source=preview; EmptyState.Visibility=Visibility.Collapsed;
                layers.Clear();
                foreach (var layer in document.Layers.Reverse())
                    layers.Add(new LayerItemViewModel { Name=layer.Name, Kind="Image", IsVisible=layer.IsVisible });
                StatusText.Text=$"{Path.GetFileName(d.FileName)} — {document.Header.Width} × {document.Header.Height}, {document.Header.Depth}-bit — {document.Layers.Count} layers";
                FitCanvas();
                return;
            }
            var b=new BitmapImage(); b.BeginInit(); b.CacheOption=BitmapCacheOption.OnLoad; b.UriSource=new Uri(d.FileName); b.EndInit(); b.Freeze(); CanvasImage.Source=b; EmptyState.Visibility=Visibility.Collapsed; layers.Clear(); layers.Add(new LayerItemViewModel { Name=Path.GetFileNameWithoutExtension(d.FileName), Kind="Image" }); StatusText.Text=$"{Path.GetFileName(d.FileName)} — {b.PixelWidth} × {b.PixelHeight}";
        } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void Tool_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tool }) {
            activeTool=tool; OptionsTitle.Text=tool;
            TypeOptions.Visibility=tool == "Type" ? Visibility.Visible : Visibility.Collapsed;
            TextCanvas.Cursor=tool == "Type" ? Cursors.IBeam : Cursors.Arrow;
        }
    }
    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (activeTool != "Type" || e.OriginalSource is TextBox) return;
        var point=e.GetPosition(TextCanvas);
        var box=new TextBox { Text="Type your text", MinWidth=120, MinHeight=34, FontSize=SelectedFontSize(), Foreground=activeTextColor, Background=Brushes.Transparent, BorderBrush=Brushes.DodgerBlue, BorderThickness=new Thickness(1), AcceptsReturn=true };
        if (FontFamilyBox.SelectedItem is string family) box.FontFamily=new FontFamily(family);
        Canvas.SetLeft(box, point.X); Canvas.SetTop(box, point.Y); TextCanvas.Children.Add(box); activeTextBox=box;
        layers.Insert(0, new LayerItemViewModel { Name="Text Layer", Kind="Text", CanvasElement=box });
        box.Focus(); box.SelectAll(); e.Handled=true;
    }
    private void TextFormat_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (activeTextBox == null) return;
        if (FontFamilyBox.SelectedItem is string family) activeTextBox.FontFamily=new FontFamily(family);
        activeTextBox.FontSize=SelectedFontSize();
    }
    private double SelectedFontSize()
    {
        var value=FontSizeBox.Text;
        if (FontSizeBox.SelectedItem is ComboBoxItem item) value=item.Content?.ToString() ?? value;
        return double.TryParse(value, out var size) ? Math.Clamp(size, 1, 1000) : 32;
    }
    private void AlignLeft_Click(object sender, RoutedEventArgs e) { if (activeTextBox != null) activeTextBox.TextAlignment=TextAlignment.Left; }
    private void AlignCenter_Click(object sender, RoutedEventArgs e) { if (activeTextBox != null) activeTextBox.TextAlignment=TextAlignment.Center; }
    private void AlignRight_Click(object sender, RoutedEventArgs e) { if (activeTextBox != null) activeTextBox.TextAlignment=TextAlignment.Right; }
    private void TextColor_Click(object sender, RoutedEventArgs e)
    {
        activeTextColor=activeTextColor == Brushes.White ? Brushes.Black : Brushes.White;
        TextColorButton.Foreground=activeTextColor;
        if (activeTextBox != null) activeTextBox.Foreground=activeTextColor;
    }
    private void Fit_Click(object sender, RoutedEventArgs e) => FitCanvas();
    private void ActualPixels_Click(object sender, RoutedEventArgs e) => SetZoom(1);
    private void ZoomIn_Click(object sender, RoutedEventArgs e) => SetZoom(zoom * 1.25);
    private void ZoomOut_Click(object sender, RoutedEventArgs e) => SetZoom(zoom / 1.25);
    private void FitCanvas()
    {
        if (CanvasImage.Source is not BitmapSource b) return;
        var availableWidth = Math.Max(100, ActualWidth - 390);
        var availableHeight = Math.Max(100, ActualHeight - 150);
        SetZoom(Math.Min(availableWidth / b.PixelWidth, availableHeight / b.PixelHeight));
    }
    private void SetZoom(double value)
    {
        zoom = Math.Clamp(value, 0.05, 32);
        CanvasImage.LayoutTransform = new System.Windows.Media.ScaleTransform(zoom, zoom);
        StatusText.Text = $"{StatusText.Text.Split(" — ")[0]} — {zoom:P0}";
    }
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();
}
