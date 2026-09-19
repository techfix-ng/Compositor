using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Compositor.Psd;
using Microsoft.Win32;

namespace Compositor.Windows;
public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var d = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff;*.psd;*.psb|All files|*.*" };
        if (d.ShowDialog(this) != true) return;
        try {
            var ext = Path.GetExtension(d.FileName).ToLowerInvariant();
            if (ext is ".psd" or ".psb") { using var s=File.OpenRead(d.FileName); var h=PsdHeaderReader.Read(s); StatusText.Text=$"{Path.GetFileName(d.FileName)} — {h.Width} × {h.Height}, {h.Depth}-bit"; LayersList.ItemsSource=new[]{"PSD structure detected"}; return; }
            var b=new BitmapImage(); b.BeginInit(); b.CacheOption=BitmapCacheOption.OnLoad; b.UriSource=new Uri(d.FileName); b.EndInit(); b.Freeze(); CanvasImage.Source=b; EmptyState.Visibility=Visibility.Collapsed; LayersList.ItemsSource=new[]{Path.GetFileNameWithoutExtension(d.FileName)}; StatusText.Text=$"{Path.GetFileName(d.FileName)} — {b.PixelWidth} × {b.PixelHeight}";
        } catch(Exception ex) { MessageBox.Show(this, ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();
}
