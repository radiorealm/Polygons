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
            drawingContext.DrawEllipse(new SolidColorBrush(c), pen, new Point(x, y), r, r);
        }

        public override bool IsUnderCursor(double x, double y)
        {
            var dx = x - this.x;
            var dy = y - this.y;
            return (dx * dx + dy * dy) <= (r * r);
        }
    }
}
