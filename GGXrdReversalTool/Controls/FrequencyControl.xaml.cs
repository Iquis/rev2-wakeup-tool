using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GGXrdReversalTool.Controls;

public partial class FrequencyControl : UserControl
{
    public FrequencyControl()
    {
        InitializeComponent();
    }


    public int Percentage
    {
        get => (int)GetValue(PercentageProperty);
        set => SetValue(PercentageProperty, value);
    }

    public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
        nameof(Percentage), typeof(int), typeof(FrequencyControl),
        new FrameworkPropertyMetadata(100, null, OnCoercePercentageProperty)
        {
            BindsTwoWayByDefault = true,
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });

    private static object OnCoercePercentageProperty(DependencyObject source, object baseValue)
    {
        return baseValue is not int value ? 
            PercentageProperty.DefaultMetadata.DefaultValue : 
            Math.Clamp(value, 0, 100);
    }

    
}