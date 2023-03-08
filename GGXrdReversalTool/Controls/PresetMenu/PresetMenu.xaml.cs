using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Presets;

namespace GGXrdReversalTool.Controls.PresetMenu;

public partial class PresetMenu : UserControl
{
    public PresetMenu()
    {
        InitializeComponent();
    }

    public IEnumerable<Preset> Presets
    {
        get => (IEnumerable<Preset>)GetValue(PresetsProperty);
        set => SetValue(PresetsProperty, value);
    }

    public static readonly DependencyProperty PresetsProperty =
        DependencyProperty.Register(nameof(Presets), typeof(IEnumerable<Preset>), typeof(PresetMenu), new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
    
    public ICommand InsertPresetInputCommand
    {
        get => (ICommand)GetValue(InsertPresetInputCommandProperty);
        set => SetValue(InsertPresetInputCommandProperty, value);
    }

    public static readonly DependencyProperty InsertPresetInputCommandProperty =
        DependencyProperty.Register(nameof(InsertPresetInputCommand), typeof(ICommand), typeof(PresetMenu));
    
    
    
    
    
    
    
    
    
    
    
    
    
    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

    // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty GroupNameProperty =
        DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(PresetMenu),
            new PropertyMetadata("GroupName"));

}