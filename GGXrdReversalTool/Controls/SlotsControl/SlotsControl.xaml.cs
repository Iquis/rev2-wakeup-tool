using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GGXrdReversalTool.Controls.SlotsControl;

public partial class SlotsControl : UserControl
{
    public SlotsControl()
    {
        InitializeComponent();
    }


    #region SlotNumber Property

    public int SlotNumber
    {
        get => (int)GetValue(SlotNumberProperty);
        set => SetValue(SlotNumberProperty, value);
    }

    // Using a DependencyProperty as the backing store for SlotNumber.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SlotNumberProperty =
        DependencyProperty.Register(nameof(SlotNumber), typeof(int), typeof(SlotsControl),
            new FrameworkPropertyMetadata(1, OnSlotNumberPropertyChanged, OnCoerceSlotNumberProperty)
            {
                BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

    private static object OnCoerceSlotNumberProperty(DependencyObject source, object baseValue)
    {
        if (baseValue is not int value)
        {
            return SlotNumberProperty.DefaultMetadata.DefaultValue;
        }

        switch (value)
        {
            case 1:
            case 2:
            case 3:
                return value;

        }

        return SlotNumberProperty.DefaultMetadata.DefaultValue;
    }

    private static void OnSlotNumberPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs eventArgs)
    {
        if (source is not SlotsControl control)
        {
            return;
        }
        
        var value = (int)eventArgs.NewValue;

        switch (value)
        {
            case 1:
                control.RadioButton1.IsChecked = true;
                break;
            case 2:
                control.RadioButton2.IsChecked = true;
                break;
            case 3:
                control.RadioButton3.IsChecked = true;
                break;
        }



    }

    private void RadioButton1_Checked(object sender, RoutedEventArgs e)
    {
        SlotNumber = 1;
    }

    private void RadioButton2_Checked(object sender, RoutedEventArgs e)
    {
        SlotNumber = 2;
    }

    private void RadioButton3_Checked(object sender, RoutedEventArgs e)
    {
        SlotNumber = 3;
    }

    #endregion

    #region GroupName Property


    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

    // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty GroupNameProperty =
        DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(SlotsControl),
            new PropertyMetadata("GroupName"));


    #endregion
}