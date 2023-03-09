using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GGXrdReversalTool.Controls;

public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();
    }

    public bool IsAsmReplayTypeChecked
    {
        get => (bool)GetValue(IsAsmReplayTypeCheckedProperty);
        set => SetValue(IsAsmReplayTypeCheckedProperty, value);
    }
    public static readonly DependencyProperty IsAsmReplayTypeCheckedProperty = DependencyProperty.Register(
        nameof(IsAsmReplayTypeChecked), typeof(bool), typeof(MainMenu), new PropertyMetadata(default(bool)));

    public bool IsKeyStrokeReplayTypeChecked
    {
        get => (bool)GetValue(IsKeyStrokeReplayTypeCheckedProperty);
        set => SetValue(IsKeyStrokeReplayTypeCheckedProperty, value);
    }
    public static readonly DependencyProperty IsKeyStrokeReplayTypeCheckedProperty = DependencyProperty.Register(
        nameof(IsKeyStrokeReplayTypeChecked), typeof(bool), typeof(MainMenu), new PropertyMetadata(default(bool)));

    public ICommand ChangeReplayTypeCommand
    {
        get => (ICommand)GetValue(ChangeReplayTypeCommandProperty);
        set => SetValue(ChangeReplayTypeCommandProperty, value);
    }

    public static readonly DependencyProperty ChangeReplayTypeCommandProperty = DependencyProperty.Register(
        nameof(ChangeReplayTypeCommand), typeof(ICommand), typeof(MainMenu), new PropertyMetadata(default(ICommand)));
    
    public bool AutoUpdate
    {
        get => (bool)GetValue(AutoUpdateProperty);
        set => SetValue(AutoUpdateProperty, value);
    }
    public static readonly DependencyProperty AutoUpdateProperty = DependencyProperty.Register(
        nameof(AutoUpdate), typeof(bool), typeof(MainMenu), new FrameworkPropertyMetadata(default(bool)){ BindsTwoWayByDefault = true});

    public ICommand CheckUpdatesCommand
    {
        get => (ICommand)GetValue(CheckUpdatesCommandProperty);
        set => SetValue(CheckUpdatesCommandProperty, value);
    }
    public static readonly DependencyProperty CheckUpdatesCommandProperty = DependencyProperty.Register(
        nameof(CheckUpdatesCommand), typeof(ICommand), typeof(MainMenu), new PropertyMetadata(default(ICommand)));

    public ICommand AboutCommand
    {
        get => (ICommand)GetValue(AboutCommandProperty);
        set => SetValue(AboutCommandProperty, value);
    }
    public static readonly DependencyProperty AboutCommandProperty =
        DependencyProperty.Register(nameof(AboutCommand), typeof(ICommand), typeof(MainMenu));

    public Window MainWindow
    {
        get => (Window)GetValue(MainWindowProperty);
        set => SetValue(MainWindowProperty, value);
    }
    public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(
        nameof(MainWindow), typeof(Window), typeof(MainMenu), new PropertyMetadata(default(Window)));


    
    public ICommand DonateCommand
    {
        get => (ICommand)GetValue(DonateCommandProperty);
        set => SetValue(DonateCommandProperty, value);
    }
    public static readonly DependencyProperty DonateCommandProperty =
        DependencyProperty.Register(nameof(DonateCommand), typeof(ICommand), typeof(MainMenu));
}