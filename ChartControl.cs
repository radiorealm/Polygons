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
using Avalonia.Media.TextFormatting;

namespace Polygons;

public class ChartControl : UserControl
{
    private List<Shape> shapes = [];
    private List<Point> points_def = [];
    private List<Point> points_and = [];

    public int chosen_chart;
    
    private double scale_y;
    private double scale_x;

    private double _def;
    private double _and;

    private double width;
    private double height;

    public override void Render(DrawingContext drawingContext)
    {
        width = Bounds.Width - 25;
        height = Bounds.Height - 25;
        
        if (points_and.Count > 1 && points_def.Count > 1)
        {
            scale_x = width / points_def.Max(point => point.X);
            switch (chosen_chart)
            {
                case 0:
                    scale_y = height / points_def.Max(point => point.Y);
                    for (int i = 0; i < points_def.Count - 1; i++)
                    {
                        drawingContext.DrawLine(new Pen(Brushes.PowderBlue),
                            new Point(points_def[i].X * scale_x, height - (points_def[i].Y * scale_y)),
                            new Point(points_def[i + 1].X * scale_x, height - (points_def[i + 1].Y * scale_y)));
                    }

                    break;
                case 1:
                    scale_y = height / points_and.Max(point => point.Y);
                    for (int i = 0; i < points_and.Count - 1; i++)
                    {
                        drawingContext.DrawLine(new Pen(Brushes.Violet),
                            new Point(points_and[i].X * scale_x, height - (points_and[i].Y * scale_y)),
                            new Point(points_and[i + 1].X * scale_x, height - (points_and[i + 1].Y * scale_y)));
                    }

                    break;
                case 2:
                    scale_y = height / points_def.Max(point => point.Y);
                    for (int i = 0; i < points_def.Count - 1; i++)
                    {
                        drawingContext.DrawLine(new Pen(Brushes.PowderBlue),
                            new Point(points_def[i].X * scale_x, height - (points_def[i].Y * scale_y)),
                            new Point(points_def[i + 1].X * scale_x, height - (points_def[i + 1].Y * scale_y)));
                        drawingContext.DrawLine(new Pen(Brushes.Violet),
                            new Point(points_and[i].X * scale_x, height - (points_and[i].Y * scale_y)),
                            new Point(points_and[i + 1].X * scale_x, height - (points_and[i + 1].Y * scale_y)));
                    }
                    break;
            }
        }
    }

    public void DrawChart()
    {
        shapes = GenerateRandomShapes(3);
        var defTime = MeasureDefTime(UpdateConvexHull);
        var andTime = MeasureDefTime(UpdateAndrew);
        
        for (int i = 3; i <= 500; i+=10)
        {
            shapes = GenerateRandomShapes(i);
            defTime = MeasureDefTime(UpdateConvexHull);
            andTime = MeasureDefTime(UpdateAndrew);

            points_def.Add(new Point(i, defTime));
            points_and.Add(new Point(i, andTime));
        }
        
        InvalidateVisual();
    }

    private List<Shape> GenerateRandomShapes(int count)
    {
        var rand = new Random();
        var shapes = new List<Shape>();
        for (var i = 0; i < count; i++) shapes.Add(new Circle(rand.Next(-1000, 1000), rand.Next(-1000, 1000)));
        return shapes;
    }

    private double MeasureDefTime(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private void UpdateConvexHull()
    {
        if (shapes.Count < 3)
            return;

        foreach (var shape in shapes)
        {
            shape.IsInside = true;
        }

        //перебираем все пары
        for (var i = 0; i < shapes.Count; i++)
        {
            for (var j = i + 1; j < shapes.Count; j++)
            {
                var upper = false; var lower = false;

                //y1 = kx1 + b and y2 = kx2 + b. k = (y1 - y2) / (x1 - x2). b = y1 - kx1.
                var k = (double)(shapes[i].Y - shapes[j].Y) / (double)(shapes[i].X - shapes[j].X);
                var b = shapes[i].Y - k * shapes[i].X;

                //для каждой пары [i; j] проверить по одну ли сторону лежат все остальные вершины
                for (var p = 0; p <= shapes.Count - 1; p++)
                {
                    if (p == j || p == i) continue;

                    if (shapes[i].X != shapes[j].X)
                    {
                        if (k * shapes[p].X + b > shapes[p].Y) { upper = true; }
                        else if (k * shapes[p].X + b < shapes[p].Y) { lower = true; }
                    }
                    else
                    {
                        if (shapes[p].X > shapes[i].X) { upper = true; }
                        else if (shapes[p].X < shapes[i].X) { lower = true; }
                    }
                }

                if (lower == upper != true)
                {
                    shapes[i].IsInside = false;
                    shapes[j].IsInside = false;
                }
            }
        }
    }

    private void UpdateAndrew()
    {
        if (shapes.Count < 3) return;

        foreach (var shape in shapes)
        {
            shape.IsInside = true;
        }

        shapes = shapes.OrderBy(s => s.X).ThenBy(s => s.Y).ToList();

        List<Shape> lowerHull = [];
        foreach (var shape in shapes)
        {
            while (lowerHull.Count >= 2 && Orientation(lowerHull[^2], lowerHull[^1], shape) < 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }
            lowerHull.Add(shape);
        }

        List<Shape> upperHull = [];
        for (var i = shapes.Count - 1; i >= 0; i--)
        {
            while (upperHull.Count >= 2 && Orientation(upperHull[^2], upperHull[^1], shapes[i]) < 0)
            {
                upperHull.RemoveAt(upperHull.Count - 1);
            }
            upperHull.Add(shapes[i]);
        }
        upperHull.RemoveAt(upperHull.Count - 1);

        List<Shape> convexHull = lowerHull.Concat(upperHull).ToList();

        foreach (var shape in convexHull)
        {
            shape.IsInside = false;
        }
    }

    private double Orientation(Shape a, Shape b, Shape c) //ориентированная площадь треугольника (это площадь этого треугольника, взятая со знаком «плюс», если вершины записаны в положительном порядке (против часовой стрелки), и со знаком «минус» — иначе.)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    }
}
