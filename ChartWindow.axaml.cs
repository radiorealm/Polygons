using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Visuals;
using Polygons.Vertices;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace Polygons;

public partial class ChartWindow : Window
{
    public ChartWindow()
    {
        InitializeComponent();
        InvalidateVisual();
    }

    private void MyChartControl_OnLoaded(object? sender, RoutedEventArgs e)
    {
        ChartControl chartControl = this.Find<ChartControl>("chart");
        chartControl.DrawPerformance();
    }

}