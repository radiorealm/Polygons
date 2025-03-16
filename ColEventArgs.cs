using System;
using Avalonia.Media;
namespace Polygons;

public delegate void ColorDelegate(object sender, ColEventArgs args);

public class ColEventArgs(Color brush_color, Color pen_color) : EventArgs
{
    public Color brush_c = brush_color;
    public Color pen_c = pen_color;
}