using System.Windows;
using System.Windows.Controls;

namespace GGXrdReversalTool.Controls;

public partial class FrameInputControl : UserControl
{
    public FrameInputControl()
    {
        InitializeComponent();
    }

    public string FrameInput
    {
        get => (string)GetValue(FrameInputProperty);
        set => SetValue(FrameInputProperty, value);
    }

    public static readonly DependencyProperty FrameInputProperty = DependencyProperty.Register(nameof(FrameInput),
        typeof(string), typeof(FrameInputControl), new PropertyMetadata(default(string)));
}