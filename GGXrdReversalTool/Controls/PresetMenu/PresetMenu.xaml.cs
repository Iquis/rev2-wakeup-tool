using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GGXrdReversalTool.Library.Presets;

namespace GGXrdReversalTool.Controls.PresetMenu;

public partial class PresetMenu : Menu
{
    public PresetMenu()
    {
        InitializeComponent();
    }

    public IEnumerable<Preset> Preset
    {
        get => (IEnumerable<Preset>)GetValue(PresetsProperty);
        set => SetValue(PresetsProperty, value);
    }

    public static readonly DependencyProperty PresetsProperty =
        DependencyProperty.Register(nameof(Preset), typeof(IEnumerable<Preset>), typeof(PresetMenu));
}