using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal abstract class Shape(double xx, double yy)
    {
        protected double x = xx, y = yy;
        protected static Color c = Colors.LavenderBlush; //TODO: ColorPicker Avalonia!!!
        protected static double r;

        protected static Brush brush;
        protected static Pen pen;

        public bool IsHeld; //держим?
        public bool IsInside; //внутри оболочки? убираются все вершины, у которых IsInside = true => нет смысла хранить отдельный масссив

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

        public Color C
        {
            get { return c; }
            set { c = value; }
        }
        
        public static double R { get { return r; } set { r = value; } }

        public virtual void Draw(DrawingContext drawingContext) { }

        public abstract bool IsUnderCursor(double x, double y);
    }
}
