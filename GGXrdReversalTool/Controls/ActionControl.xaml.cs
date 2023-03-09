using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GGXrdReversalTool.Library.Presets;

namespace GGXrdReversalTool.Controls;

public partial class ActionControl : UserControl
{
    public ActionControl()
    {
        InitializeComponent();
    }


    public string RawInputText
    {
        get => (string)GetValue(RawInputTextProperty);
        set => SetValue(RawInputTextProperty, value);
    }
    public static readonly DependencyProperty RawInputTextProperty = DependencyProperty.Register(
        nameof(RawInputText), typeof(string), typeof(ActionControl), new FrameworkPropertyMetadata(default(string))
        {
            BindsTwoWayByDefault = true
        });


    public IEnumerable<Preset> Presets
    {
        get => (IEnumerable<Preset>)GetValue(PresetsProperty);
        set => SetValue(PresetsProperty, value);
    }
    public static readonly DependencyProperty PresetsProperty = DependencyProperty.Register(
        nameof(Presets), typeof(IEnumerable<Preset>), typeof(ActionControl), new PropertyMetadata(default(IEnumerable<Preset>)));

    public ICommand InsertPresetInputCommand
    {
        get => (ICommand)GetValue(InsertPresetInputCommandProperty);
        set => SetValue(InsertPresetInputCommandProperty, value);
    }
    public static readonly DependencyProperty InsertPresetInputCommandProperty = DependencyProperty.Register(
        nameof(InsertPresetInputCommand), typeof(ICommand), typeof(ActionControl), new PropertyMetadata(default(ICommand)));
}