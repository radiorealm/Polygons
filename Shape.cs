using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal abstract class Shape
    {
        protected double x, y;
        protected static Color c = Colors.LavenderBlush; //TODO: ColorPicker Avalonia!!!
        protected static int r;

        protected static Brush brush;
        protected static Pen pen;

        public bool IsHeld; //держим?
        public bool IsInside; //внутри оболочки? убираются все вершины, у которых IsInside = true => нет смысла хранить отдельный масссив

        public Shape(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        static Shape()
        {
            r = 20;
            brush = new SolidColorBrush(c);
            pen = new Pen(Brushes.PaleVioletRed, 2, lineCap: PenLineCap.Square);
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        Color C
        {
            get { return c; }
            set { c = value; }
        }

        public virtual void Draw(DrawingContext drawingContext) { }

        public abstract bool IsUnderCursor(double x, double y);
    }
}
