using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Visuals;
using Polygons.Vertices;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;


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
            this.ColorChanged(sender, new ColEventArgs(BrushColorPicker.Color, PenColorPicker.Color));
        }
    }

    public event ColorDelegate ColorChanged;

    public void SetColor(Color brush_color, Color pen_color)
    {
        BrushColorPicker.Color = brush_color;
        PenColorPicker.Color = pen_color;
    }
}