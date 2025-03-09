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
        public MainWindow()
        {
            InitializeComponent();

            Shapes.ItemsSource = new[] { "Circle", "Triangle", "Square" };
            Shapes.SelectedIndex = 0;

            Alg.ItemsSource = new[] { "By definition", "Andrew", "Show Comparison" };
            Alg.SelectedIndex = 0;

            var window = new RadiusWindow();
            window.Show();
        }

        public void Win_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            CustomControl custom = this.Find<CustomControl>("cc");

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
            CustomControl custom = this.Find<CustomControl>("cc");
            custom.Moved(e.GetPosition(custom).X, e.GetPosition(custom).Y);
        }

        public void Win_PointerReleased(object sender, PointerReleasedEventArgs e) 
        {
            CustomControl custom = this.Find<CustomControl>("cc");
            custom.Released(e.GetPosition(custom).X, e.GetPosition(custom).Y);
        }

        private void Shapes_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            CustomControl? customControl = this.Find<CustomControl>("cc");

            int type = Shapes.SelectedIndex;
            customControl?.ChangeShape(type);
        }

        private void Alg_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            CustomControl? customControl = this.Find<CustomControl>("cc");

            int type = Alg.SelectedIndex;
            customControl?.ChangeAlg(type);
        }

        public void OnChartClicked(object? sender, RoutedEventArgs e)
        {
            var window = new ChartWindow();
            window.Show();
        }
    }
}