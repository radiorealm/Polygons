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
    private List<Shape> shapes = new List<Shape>();
    private List<Point> points_def = new List<Point>();
    private List<Point> points_and = new List<Point>();

    public override void Render(DrawingContext drawingContext)
    {
        if (points_def.Count > 1 && points_and.Count > 1)
        {
            //double maxTime = points_def.Max(p => p.X);
            //double maxShapes = points_def.Max(p => p.Y);

            double maxTime = points_and.Max(p => p.X);
            double maxShapes = points_and.Max(p => p.Y);

            //определяем отношения
            double width = Bounds.Width - 100;
            double height = Bounds.Height - 100;

            double xScale = width / maxTime;
            double yScale = height / maxShapes;

            var transformedPointsDef = points_def.Select(p => new Point(p.X * xScale + 50, height - (p.Y * yScale) + 50)).ToList();
            var transformedPointsAnd = points_and.Select(p => new Point(p.X * xScale + 50, height - (p.Y * yScale) + 50)).ToList();

            //оси
            drawingContext.DrawLine(new Pen(Brushes.Black, 2), new Point(50, 50), new Point(50, 650)); // Ось Y
            drawingContext.DrawLine(new Pen(Brushes.Black, 2), new Point(50, 650), new Point(width + 50, 650)); // Ось X

            drawingContext.DrawGeometry(new SolidColorBrush(Colors.HotPink), new Pen(Brushes.HotPink, 2), new PolylineGeometry(transformedPointsDef, false));
            drawingContext.DrawGeometry(new SolidColorBrush(Colors.CornflowerBlue), new Pen(Brushes.CornflowerBlue, 2), new PolylineGeometry(transformedPointsAnd, false));
        }
    }

    public void DrawPerformance()
    {
        for (int i = 100; i < 1500; i = i + 100)
        {
            shapes = GenerateRandomShapes(i);
            points_def.Add(new Point(i, MeasureDefTime(UpdateConvexHull)));
            //points_def.Add(new Point(0, 0));
            points_and.Add(new Point(i, MeasureDefTime(UpdateAndrew)));
        }

        InvalidateVisual();
    }

    private List<Shape> GenerateRandomShapes(int count)
    {
        var rand = new Random();
        var shapes = new List<Shape>();
        for (int i = 0; i < count; i++) shapes.Add(new Circle(rand.Next(-1000, 1000), rand.Next(-1000, 1000)));
        return shapes;
    }

    public double MeasureDefTime(Action action)
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
        for (int i = 0; i < shapes.Count; i++)
        {
            for (int j = i + 1; j < shapes.Count; j++)
            {
                bool upper = false; bool lower = false;

                //y1 = kx1 + b and y2 = kx2 + b. k = (y1 - y2) / (x1 - x2). b = y1 - kx1.
                double k = (double)(shapes[i].Y - shapes[j].Y) / (double)(shapes[i].X - shapes[j].X);
                double b = shapes[i].Y - k * shapes[i].X;

                //для каждой пары [i; j] проверить по одну ли сторону лежат все остальные вершины
                for (int p = 0; p <= shapes.Count - 1; p++)
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

                if (!(lower == upper == true))
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

        List<Shape> lowerHull = new();
        foreach (var shape in shapes)
        {
            while (lowerHull.Count >= 2 && Orientation(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], shape) < 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }
            lowerHull.Add(shape);
        }

        List<Shape> upperHull = new();
        for (int i = shapes.Count - 1; i >= 0; i--)
        {
            while (upperHull.Count >= 2 && Orientation(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], shapes[i]) < 0)
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
