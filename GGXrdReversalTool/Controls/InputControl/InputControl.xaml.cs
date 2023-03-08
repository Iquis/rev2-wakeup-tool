using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GGXrdReversalTool.Controls.InputControl;

public partial class InputControl : ContentControl
{
    public InputControl()
    {
        InitializeComponent();
    }

    public IEnumerable<string> Inputs { get; set; }
    
    public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(nameof(Inputs), typeof(IEnumerable<string>), typeof(InputControl));
}