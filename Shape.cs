using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    public abstract class Shape(double xx, double yy)
    {
        protected double x = xx, y = yy;
        protected static Color brush_c;
        protected static Color pen_c;
        protected static double r;

        protected static Brush brush;
        protected static Pen pen;

        public bool IsHeld; //держим?
        public bool IsInside; //внутри оболочки? убираются все вершины, у которых IsInside = true => нет смысла хранить отдельный масссив

        static Shape()
        {
            r = 20;
            pen_c = Colors.LavenderBlush;
            brush_c = Colors.CornflowerBlue;
            brush = new SolidColorBrush(brush_c);
            pen = new Pen(new SolidColorBrush(pen_c), 2);
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

        public static Color Brush_C { get { return brush_c; } set { brush_c = value; brush = new SolidColorBrush(brush_c); } }
        
        public static Color Pen_C { get { return pen_c; } set { pen_c = value; pen = new Pen(new SolidColorBrush(pen_c), 2); } }
        
        public static double R { get { return r; } set { r = value; } }

        public virtual void Draw(DrawingContext drawingContext) { }

        public abstract bool IsUnderCursor(double x, double y);
    }
}
