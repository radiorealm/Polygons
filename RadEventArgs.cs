using System;

namespace Polygons;

public delegate void RadiusDelegate(object sender, RadEventArgs args);

public class RadEventArgs(double r) : EventArgs
{
    public double r = r;
}