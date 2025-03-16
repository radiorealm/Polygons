using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Skia;
using Polygons.Vertices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Polygons
{
    public partial class MainWindow : Window
    {
        private RadiusWindow? radWindow;
        private ColorWindow? colWindow;
        
        public MainWindow()
        {
            InitializeComponent();

            Shapes.ItemsSource = new[] { "Circle", "Triangle", "Square" };
            Shapes.SelectedIndex = 0;

            Alg.ItemsSource = new[] { "By definition", "Andrew" };
            Alg.SelectedIndex = 0;
        }

        public void Win_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var custom = this.Find<CustomControl>("cc");

            if (e.GetCurrentPoint(custom).Properties.IsLeftButtonPressed)
            {
                custom.LeftPressed(e.GetPosition(custom).X, e.GetPosition(custom).Y);
            }
            else if (e.GetCurrentPoint(custom).Properties.IsRightButtonPressed)
            {
                custom.RightPressed(e.GetPosition(custom).X, e.GetPosition(custom).Y);
            }
        }

        public void Win_PointerMoved (object sender, PointerEventArgs e)
        {
            var custom = this.Find<CustomControl>("cc");
            custom.Moved(e.GetPosition(custom).X, e.GetPosition(custom).Y);
        }

        public void Win_PointerReleased(object sender, PointerReleasedEventArgs e) 
        {
            var custom = this.Find<CustomControl>("cc");
            custom.Released(e.GetPosition(custom).X, e.GetPosition(custom).Y);
        }

        private void Shapes_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var customControl = this.Find<CustomControl>("cc");

            var type = Shapes.SelectedIndex;
            customControl?.ChangeShape(type);
        }

        private void Alg_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var customControl = this.Find<CustomControl>("cc");

            var type = Alg.SelectedIndex;
            customControl?.ChangeAlg(type);
        }

        public void OnChartClicked(object? sender, RoutedEventArgs e)
        {
            var window = new ChartWindow();
            window.Show();
        }

        private void OnRadiusChanged(object? sender, RoutedEventArgs e)
        {
            var customControl = this.Find<CustomControl>("cc");
            
            if (radWindow == null)
            {
                radWindow = new RadiusWindow();
                radWindow.SetRadius(Shape.R);
                radWindow.RadiusChanged += customControl!.UpdateRadius;

                radWindow.Closed += (s, args) => radWindow = null;
                radWindow.Show();
            }
            else
            {
                radWindow.Activate();
            }
        }
        
        private void OnColorChanged(object? sender, RoutedEventArgs e)
        {
            var customControl = this.Find<CustomControl>("cc");
            
            if (colWindow == null)
            {
                colWindow = new ColorWindow();
                colWindow.SetColor(Shape.C);
                colWindow.ColorChanged += customControl!.UpdateColor;

                colWindow.Closed += (s, args) => colWindow = null;
                colWindow.Show();
            }
            else
            {
                colWindow.Activate();
            }
        }
    }
}