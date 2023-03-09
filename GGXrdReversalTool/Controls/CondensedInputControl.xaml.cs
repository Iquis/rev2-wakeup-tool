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

    //TODO Implement

    // public IEnumerable ItemsSource
    // {
    //     get => (IEnumerable)GetValue(TestProperty);
    //     set => SetValue(TestProperty);
    // }
    //
    // public static readonly DependencyProperty TestProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IEnumerable), typeof (CondensedInputControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback((object) null, OnItemsSourceChanged)));
}