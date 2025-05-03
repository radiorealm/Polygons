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
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Polygons
{
    public partial class MainWindow : Window
    {
        private CustomWindow? radWindow;
        
        private string filePath;
        
        public MainWindow()
        {
            InitializeComponent();

            Shapes.ItemsSource = new[] { "Circle", "Triangle", "Square" };
            Shapes.SelectedIndex = 0;

            Alg.ItemsSource = new[] { "By definition", "Andrew" };
            Alg.SelectedIndex = 1;
            
            Chart.ItemsSource = new[] { "By definition", "Andrew", "Both" };
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
            window.Chart_ = Chart.SelectedIndex;
            window.Show();
        }

        private void OnCustomization(object? sender, RoutedEventArgs e)
        {
            var customControl = this.Find<CustomControl>("cc");
            
            if (radWindow == null)
            {
                radWindow = new CustomWindow();
                radWindow.SetRadius(Shape.R);
                radWindow.RadiusChanged += customControl!.UpdateRadius;
                
                radWindow.SetColor(Shape.Brush_C, Shape.Pen_C);
                radWindow.ColorChanged += customControl!.UpdateColor;

                radWindow.Closed += (s, args) => radWindow = null;
                radWindow.Show();
            }
            else
            {
                radWindow.Activate();
            }
        }
        
        //-------------------------------------

        private async void OnNewClick(object? sender, RoutedEventArgs routedEventArgs)
        {
            Console.WriteLine("New");
            var custom = this.Find<CustomControl>("cc");
            
            if (!custom.IsChanged || await ConfirmChanges())
            {
                custom.LoadShapes(new List<Data>());
                filePath = null;
            }
        }

        private async void OnOpenClick(object? sender, RoutedEventArgs routedEventArgs)
        {
            Console.WriteLine("Open");
            var custom = this.Find<CustomControl>("cc");

            if (!custom.IsChanged || await ConfirmChanges())
            {

                var dialog = new OpenFileDialog
                {
                    AllowMultiple = false
                };

                var result = await dialog.ShowAsync(this);

                if (result != null && result.Length > 0)
                {
                    filePath = result[0];
                    var json = await File.ReadAllTextAsync(filePath);
                    custom.LoadShapes(JsonSerializer.Deserialize<List<Data>>(json));
                }
            }
        }

        private async void OnSaveClick(object? sender, RoutedEventArgs routedEventArgs)
        {
            Console.WriteLine("Save");
            if (filePath != null)
            {
                var custom = this.Find<CustomControl>("cc");
                var shapes = custom.SaveShapes();
                var json = JsonSerializer.Serialize(shapes, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
            }

            else
            {
                OnSaveAsClick(null, null);
            }
        }

        private async void OnSaveAsClick(object? sender, RoutedEventArgs routedEventArgs)
        {
            Console.WriteLine("Save as");
            var dialog = new SaveFileDialog
            {
                DefaultExtension = "json"
            };

            var result = await dialog.ShowAsync(this);

            if (!string.IsNullOrEmpty(result))
            {
                filePath = result;
                OnSaveClick(null, null);
            }
        }

        private async Task<bool> ConfirmChanges() //true = открыть (новый) файл, false = отмена операции
        {
            var custom = this.Find<CustomControl>("cc");
            if (custom.IsChanged)
            {
                var window = new ChangedWindow();
                var result = await window.ShowDialog<string>(this);
                Console.WriteLine(result);

                switch (result)
                {
                    case "Save":
                        OnSaveClick(null, null);
                        return true;
                    case "DontSave":
                        return true;
                    case "Cancel":
                        return false;
                }
            }
            return true;
        }
    }
}