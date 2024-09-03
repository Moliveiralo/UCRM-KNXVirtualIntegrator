﻿using System.Windows;
using System.Windows.Input;

namespace KNX_Virtual_Integrator.View.Windows;

public partial class ReportCreationWindow : Window
{
    public ReportCreationWindow()
    {
        InitializeComponent();
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        DragMove();
    }

    private void CancelButtonClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}