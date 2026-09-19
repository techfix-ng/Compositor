using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Compositor.Psd;
using Microsoft.Win32;
using System.Windows.Controls;

namespace Compositor.Windows;
public partial class MainWindow : Window
{
    private double zoom = 1;
    public MainWindow() => InitializeComponent();
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
                LayersList.ItemsSource=document.Layers.Select(l => $"☑  {l.Name}").Reverse().ToArray();
                StatusText.Text=$"{Path.GetFileName(d.FileName)} — {document.Header.Width} × {document.Header.Height}, {document.Header.Depth}-bit — {document.Layers.Count} layers";
                FitCanvas();
                return;
            }
            var b=new BitmapImage(); b.BeginInit(); b.CacheOption=BitmapCacheOption.OnLoad; b.UriSource=new Uri(d.FileName); b.EndInit(); b.Freeze(); CanvasImage.Source=b; EmptyState.Visibility=Visibility.Collapsed; LayersList.ItemsSource=new[]{Path.GetFileNameWithoutExtension(d.FileName)}; StatusText.Text=$"{Path.GetFileName(d.FileName)} — {b.PixelWidth} × {b.PixelHeight}";
        } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void Tool_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tool }) OptionsTitle.Text = tool;
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
