using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Avalonia;
using Avalonia.Media;
namespace Polygons;

[Serializable]
public class Data
{
    public string Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    
    public double Radius { get; set; }

    [JsonConstructor]
    public Data(string type, double x, double y, double radius)
    {
        Type = type;
        X = x;
        Y = y;
        Radius = radius;
    }
}