using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using GGXrdReversalTool.Library.Scenarios.Action;

namespace GGXrdReversalTool.Controls;

public partial class EventControl : UserControl, INotifyPropertyChanged
{
    public EventControl()
    {
        InitializeComponent();
    }

    public IEnumerable<ScenarioActionTypes> ActionTypes
    {
        get => (IEnumerable<ScenarioActionTypes>)GetValue(ActionTypesProperty);
        set => SetValue(ActionTypesProperty, value);
    }
    public static readonly DependencyProperty ActionTypesProperty = DependencyProperty.Register(
        nameof(ActionTypes), typeof(IEnumerable<ScenarioActionTypes>), typeof(EventControl), new PropertyMetadata(default(IEnumerable<ScenarioActionTypes>)));

    private ScenarioActionTypes? _selectedScenarioEvent;
    public ScenarioActionTypes? SelectedScenarioEvent
    {
        get => _selectedScenarioEvent;
        set
        {
            if (value == _selectedScenarioEvent) return;
            
            _selectedScenarioEvent = value;
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedScenarioEvent)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    public int MinComboCount
    {
        get => (int)GetValue(MinComboCountProperty);
        set => SetValue(MinComboCountProperty, value);
    }
    public static readonly DependencyProperty MinComboCountProperty = DependencyProperty.Register(
        nameof(MinComboCount), typeof(int), typeof(EventControl), new PropertyMetadata(1));

    public int MaxComboCount
    {
        get => (int)GetValue(MaxComboCountProperty);
        set => SetValue(MaxComboCountProperty, value);
    }
    public static readonly DependencyProperty MaxComboCountProperty = DependencyProperty.Register(
        nameof(MaxComboCount), typeof(int), typeof(EventControl), new PropertyMetadata(100));


    public bool ShouldCheckWakingUp
    {
        get => (bool)GetValue(ShouldCheckWakingUpProperty);
        set => SetValue(ShouldCheckWakingUpProperty, value);
    }
    public static readonly DependencyProperty ShouldCheckWakingUpProperty = DependencyProperty.Register(
        nameof(ShouldCheckWakingUp), typeof(bool), typeof(EventControl), new PropertyMetadata(true));

    public bool ShouldCheckWallSplat
    {
        get => (bool)GetValue(ShouldCheckWallSplatProperty);
        set => SetValue(ShouldCheckWallSplatProperty, value);
    }
    public static readonly DependencyProperty ShouldCheckWallSplatProperty = DependencyProperty.Register(
        nameof(ShouldCheckWallSplat), typeof(bool), typeof(EventControl), new PropertyMetadata(default(bool)));

    public bool ShouldCheckAirTech
    {
        get => (bool)GetValue(ShouldCheckAirTechProperty);
        set => SetValue(ShouldCheckAirTechProperty, value);
    }
    public static readonly DependencyProperty ShouldCheckAirTechProperty = DependencyProperty.Register(
        nameof(ShouldCheckAirTech), typeof(bool), typeof(EventControl), new PropertyMetadata(default(bool)));

    public bool ShouldCheckStartBlocking
    {
        get => (bool)GetValue(ShouldCheckStartBlockingProperty);
        set => SetValue(ShouldCheckStartBlockingProperty, value);
    }

    public static readonly DependencyProperty ShouldCheckStartBlockingProperty = DependencyProperty.Register(
        nameof(ShouldCheckStartBlocking), typeof(bool), typeof(EventControl), new PropertyMetadata(default(bool)));
}

public class EventControlDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate ComboDataTemplate { get; set; } = null!;
    public DataTemplate AnimationDataTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ScenarioActionTypes actionType)
        {
            return actionType switch
            {
                ScenarioActionTypes.Animation => AnimationDataTemplate,
                ScenarioActionTypes.Combo => ComboDataTemplate,
                _ => new DataTemplate()
            };
        }

        return new DataTemplate();
    }
}