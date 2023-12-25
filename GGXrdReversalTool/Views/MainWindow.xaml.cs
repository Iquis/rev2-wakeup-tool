using System;
using System.Windows;
using GGXrdReversalTool.ViewModels;

namespace GGXrdReversalTool.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is not ScenarioWindowViewModel scenarioWindowViewModel) return;
        if (!scenarioWindowViewModel.DisableCommand.CanExecute()) return;
        scenarioWindowViewModel.DisableCommand.Execute();
    }
}