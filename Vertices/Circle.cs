using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Polygons.Vertices
{
    class Circle : Shape
    {
        public Circle(double x, double y) : base(x, y) { }

        public bool IsInside { get; set; } = false;
        public override void Draw(DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(brush, pen, new Point(x, y), r, r);
        }

        public override bool IsUnderCursor(double x, double y)
        {
            double dx = x - this.x;
            double dy = y - this.y;
            return (dx * dx + dy * dy) <= (r * r);
        }
    }
}
