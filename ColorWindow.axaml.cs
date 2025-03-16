using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;

namespace Polygons;

public partial class ColorWindow : Window
{
    public ColorWindow()
    {
        InitializeComponent();
    }

    private void ColorView_OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        if (this.ColorChanged != null)
        {
            this.ColorChanged(sender, new ColEventArgs(ColorPicker.Color));
        }
    }

    public event ColorDelegate ColorChanged;

    public void SetColor(Color color)
    {
        ColorPicker.Color = color;
    }
}