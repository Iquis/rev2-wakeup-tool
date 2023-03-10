using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GGXrdReversalTool.Library.Presets;

namespace GGXrdReversalTool.Controls;

public partial class PresetMenu : UserControl
{
    public PresetMenu()
    {
        InitializeComponent();
    }

    public IEnumerable<Preset> Presets => Preset.Presets;
    
    public ICommand InsertPresetInputCommand
    {
        get => (ICommand)GetValue(InsertPresetInputCommandProperty);
        set => SetValue(InsertPresetInputCommandProperty, value);
    }

    public static readonly DependencyProperty InsertPresetInputCommandProperty =
        DependencyProperty.Register(nameof(InsertPresetInputCommand), typeof(ICommand), typeof(PresetMenu));

}