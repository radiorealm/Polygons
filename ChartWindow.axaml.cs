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
    private int chosen_chart;
    
    public ChartWindow()
    {
        InitializeComponent();
        InvalidateVisual();
    }

    public int Chart_
    {
        set { chosen_chart = value;} 
    }
    
    private void MyChartControl_OnLoaded(object? sender, RoutedEventArgs e)
    {
        var chartControl = this.Find<ChartControl>("chart");
        chartControl.chosen_chart = chosen_chart;
        chartControl.DrawChart();
    }

}