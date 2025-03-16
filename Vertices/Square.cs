using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Polygons.Vertices
{
    class Square : Shape
    {
        private double side => Math.Sqrt(2) * r;
        public bool IsInside { get; set; } = false;

        public Square(double x, double y) : base(x, y) { }

        public override void Draw(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(c), pen, new Rect(x - side / 2, y - side / 2, side, side));
        }

        public override bool IsUnderCursor(double x, double y)
        {
            return x >= this.x - side / 2 && x <= this.x + side / 2 &&
                   y >= this.y - side / 2 && y <= this.y + side / 2;
        }
    }
}