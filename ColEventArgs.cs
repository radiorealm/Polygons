using System;
using Avalonia.Media;
namespace Polygons;

public delegate void ColorDelegate(object sender, ColEventArgs args);

public class ColEventArgs(Color color) : EventArgs
{
    public Color c = color;
}