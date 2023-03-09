using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GGXrdReversalTool.Controls;

public partial class EnableDisableScenarioControl : UserControl
{
    public EnableDisableScenarioControl()
    {
        InitializeComponent();
    }

    public ICommand EnableCommand
    {
        get => (ICommand)GetValue(EnableCommandProperty);
        set => SetValue(EnableCommandProperty, value);
    }
    public static readonly DependencyProperty EnableCommandProperty = DependencyProperty.Register(
        nameof(EnableCommand), typeof(ICommand), typeof(EnableDisableScenarioControl), new PropertyMetadata(default(ICommand)));

    public ICommand DisableCommand
    {
        get => (ICommand)GetValue(DisableCommandProperty);
        set => SetValue(DisableCommandProperty, value);
    }

    public static readonly DependencyProperty DisableCommandProperty = DependencyProperty.Register(
        nameof(DisableCommand), typeof(ICommand), typeof(EnableDisableScenarioControl), new PropertyMetadata(default(ICommand)));
}