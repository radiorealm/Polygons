using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Avalonia;
using Polygons.Vertices;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using SkiaSharp;

namespace Polygons
{
    public class CustomControl : UserControl
    {
        private List<Shape> shapes = new();
        private List<Data> data = new();

        private int chosen_shape;
        private int chosen_alg;

        private double prev_x, prev_y;

        private bool IsHolding;
        
        public bool IsChanged { get; set; } = false;

        public override void Render(DrawingContext drawingContext)
        {
            if (shapes.Count >= 3)
            {
                switch (chosen_alg)
                {
                    case 0:
                        ConvexByDef(drawingContext);
                        break;
                    case 1:
                        Andrew(drawingContext);
                        break;
                }
            }

            foreach (var shape in shapes)
            {
                shape.Draw(drawingContext);
            }
        }

        public void LeftPressed(double x, double y)
        {
            foreach (var shape in shapes.Where(shape => shape.IsUnderCursor(x, y)))
            {
                shape.IsHeld = true;
                IsHolding = true;
                prev_x = x; prev_y = y;
            }

            if (!IsHolding)
            {
                if (!IsPointInsideConvexHull(x, y))
                {
                    switch (chosen_shape)
                    {
                        case 0:
                            shapes.Add(new Circle(x, y));
                            break;

                        case 1:
                            shapes.Add(new Triangle(x, y));
                            break;

                        case 2:
                            shapes.Add(new Square(x, y));
                            break;
                    }
                }

                else
                {
                    foreach (var shape in shapes)
                    {
                        shape.IsHeld = true;
                    }

                    prev_x = x; prev_y = y;
                }
            }
            
            IsChanged = true;
            InvalidateVisual();
        }

        public void RightPressed(double x, double y)
        {
            var remove = shapes.LastOrDefault(shape => shape.IsUnderCursor(x, y));

            if (remove != null)
            {
                shapes.Remove(remove);
                IsChanged = true;
            }
            else if (IsPointInsideConvexHull(x, y))
            {
                shapes.Clear();
                IsChanged = true;
            }

            InvalidateVisual();
        }

        public void Moved(double x, double y)
        {
            foreach (var shape in shapes)
            {
                if (shape.IsHeld)
                {
                    shape.X += x - prev_x;
                    shape.Y += y - prev_y;
                }
            }
            prev_x = x; prev_y = y;
            
            InvalidateVisual();
        }

        public void Released(double x, double y)
        {
            foreach (var shape in shapes)
            {
                shape.IsHeld = false;
            }

            IsHolding = false;

            RemoveInsideShapes();
            InvalidateVisual();
        }

        public void ConvexByDef(DrawingContext drawingContext)
        {
            if (shapes.Count < 3) return;

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

                    if (!(lower == upper == true))
                    {
                        Brush lineBrush = new SolidColorBrush(Colors.BlueViolet);
                        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);

                        shapes[i].IsInside = false;
                        shapes[j].IsInside = false;
                        drawingContext.DrawLine(pen, new Point(shapes[i].X, shapes[i].Y), new Point(shapes[j].X, shapes[j].Y));
                    }
                }
            }
        }

        public void Andrew(DrawingContext drawingContext)
        {
            if (shapes.Count < 3) return;

            foreach (var shape in shapes)
            {
                shape.IsInside = true;
            }

            // Сортируем по X, затем по Y
            shapes = shapes.OrderBy(s => s.X).ThenBy(s => s.Y).ToList();

            List<Shape> lowerHull = [];
            foreach (var shape in shapes)
            {
                while (lowerHull.Count >= 2 && Orientation(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], shape) < 0)
                {
                    lowerHull.RemoveAt(lowerHull.Count - 1);
                }
                lowerHull.Add(shape);
            }

            List<Shape> upperHull = [];
            for (var i = shapes.Count - 1; i >= 0; i--)
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

            Brush lineBrush = new SolidColorBrush(Colors.Goldenrod);
            Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);

            for (var i = 0; i < convexHull.Count - 1; i++)
            {
                drawingContext.DrawLine(pen, new Point(convexHull[i].X, convexHull[i].Y), new Point(convexHull[i + 1].X, convexHull[i + 1].Y));
            }

            drawingContext.DrawLine(pen, new Point(convexHull.Last().X, convexHull.Last().Y), new Point(convexHull.First().X, convexHull.First().Y));
        }

        public bool IsPointInsideConvexHull(double x, double y)
        {
            if (shapes.Count >= 3)
            {
                for (var i = 0; i < shapes.Count - 1; i++)
                {
                    var upper = false; var lower = false;

                    var k = (double)(y - shapes[i].Y) / (double)(x - shapes[i].X);
                    var b = shapes[i].Y - shapes[i].X * k;

                    for (var l = 0; l < shapes.Count; l++)
                    {
                        if ((l != i))
                        {
                            if (shapes[i].X != x)
                            {
                                if (k * shapes[l].X + b > shapes[l].Y) { upper = true; }
                                else if (k * shapes[l].X + b < shapes[l].Y) { lower = true; }
                            }
                            else
                            {
                                if (shapes[l].X > shapes[i].X) { upper = true; }
                                else if (shapes[l].X < shapes[i].X) { lower = true; }
                            }
                        }
                    }

                    if (!(lower == upper == true))
                    {
                        return false;
                    }
                }
            }
            else { return false; }

            return true;
        }

        private double Orientation(Shape a, Shape b, Shape c) //ориентированная площадь треугольника (это площадь этого треугольника, взятая со знаком «плюс», если вершины записаны в положительном порядке (против часовой стрелки), и со знаком «минус» — иначе.)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        private void RemoveInsideShapes()
        {
            shapes.RemoveAll(shape => shape.IsInside);
        }

        public void ChangeShape(int type)
        {
            chosen_shape = type;
        }

        public void ChangeAlg(int type)
        {
            chosen_alg = type;
        }

        //-----------------------------------------------

        public void UpdateRadius(object sender, RadEventArgs e)
        {
            Shape.R = e.r;
            IsChanged = true;
            InvalidateVisual();
        }
        
        public void UpdateColor(object sender, ColEventArgs e)
        {
            Shape.Brush_C = e.brush_c;
            Shape.Pen_C = e.pen_c;
            InvalidateVisual();
        }
        
        //-----------------------------------------------

        public List<Data> SaveShapes()
        {
            IsChanged = false;
            foreach (var shape in shapes)
            {
                data.Add(new Data(shape.GetType().Name, shape.X, shape.Y, Shape.R));
            }
            return data;
        }
        
        public async void LoadShapes(List<Data> _data)
        {
            data.Clear();
            shapes.Clear();
            
            foreach (Data d in _data)
            {
                Shape.R = _data[0].Radius;
                switch (d.Type)
                {
                    case "Circle":
                        shapes.Add(new Circle(d.X, d.Y));
                        break;
                    case "Square":
                        shapes.Add(new Square(d.X, d.Y));
                        break;
                    case "Triangle":
                        shapes.Add(new Triangle(d.X, d.Y));
                        break;
                }
            }
            
            InvalidateVisual();
        }
    }
}