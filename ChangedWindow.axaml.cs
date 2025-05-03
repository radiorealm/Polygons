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

public partial class ChangedWindow : Window
{
    
    public ChangedWindow()
    {
        InitializeComponent();
        InvalidateVisual();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        Close("Save");
    }

    private void OnDontSaveClick(object sender, RoutedEventArgs e)
    {
        Close("DontSave");
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        Close("Cancel");
    }
}