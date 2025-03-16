using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Avalonia;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons.Vertices
{
    class Triangle : Shape
    {
        private Point A => new Point(x, y - r);
        private Point B => new Point(x + (Math.Sqrt(3) / 2) * r, y + r / 2);
        private Point C => new Point(x - (Math.Sqrt(3) / 2) * r, y + r / 2);

        public bool IsInside { get; set; } = false;

        public Triangle(double x, double y) : base(x, y) { }

        public override void Draw(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(brush, pen, new PolylineGeometry([A, B, C, A], true));
        }

        public override bool IsUnderCursor(double x, double y)
        {
            double Area(Point p1, Point p2, Point p3)
            {
                return Math.Abs((p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y)) / 2.0);
            }

            var areaABC = Area(A, B, C);
            var areaPAB = Area(new Point(x, y), A, B);
            var areaPBC = Area(new Point(x, y), B, C);
            var areaPCA = Area(new Point(x, y), C, A);

            return Math.Abs(areaABC - (areaPAB + areaPBC + areaPCA)) < 0.01;
        }
    }
}
