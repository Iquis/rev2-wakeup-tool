﻿using System.Windows;
using GGXrdReversalTool.ViewModels;

namespace GGXrdReversalTool;

public partial class AboutWindow : Window
{
    public AboutWindow(bool offlineMode = false)
    {
        InitializeComponent();

        if (DataContext is not AboutViewModel aboutViewModel) return;
        
        aboutViewModel.OfflineMode = offlineMode;
    }
}