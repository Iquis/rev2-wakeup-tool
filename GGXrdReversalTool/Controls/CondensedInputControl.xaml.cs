using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Controls;

public partial class CondensedInputControl : ItemsControl
{
    public CondensedInputControl()
    {
        InitializeComponent();
    }

    public IEnumerable<CondensedInput> CondensedInputs
    {
        get => (IEnumerable<CondensedInput>)GetValue(CondensedInputsProperty);
        set => SetValue(CondensedInputsProperty, value);
    }

    public static readonly DependencyProperty CondensedInputsProperty =
        DependencyProperty.Register(nameof(CondensedInputs), typeof(IEnumerable<CondensedInput>),
            typeof(CondensedInputControl), new PropertyMetadata(Enumerable.Empty<CondensedInput>()));
}

public class CondensedInputTemplateSelector : DataTemplateSelector
{
    public DataTemplate MultipleMultiplicator { get; set; } = null!;

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        return item is <= 1 ? new DataTemplate() : MultipleMultiplicator;
    }
} 