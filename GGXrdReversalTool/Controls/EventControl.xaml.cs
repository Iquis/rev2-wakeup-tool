using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;

namespace GGXrdReversalTool.Controls;

public sealed partial class EventControl
{
    public EventControl()
    {
        InitializeComponent();
    }

    public IEnumerable<ScenarioEventTypes> ActionTypes => Enum.GetValues<ScenarioEventTypes>();

    private ScenarioEventTypes? _selectedScenarioEvent;
    public ScenarioEventTypes? SelectedScenarioEvent
    {
        get => _selectedScenarioEvent;
        set
        {
            if (value == _selectedScenarioEvent) return;
            
            _selectedScenarioEvent = value;
            
            OnPropertyChanged();
            
            CreateScenario();
        }
    }

    private int _minComboCount = 1;
    public int MinComboCount
    {
        get => _minComboCount;
        set
        {
            var coercedValue = Math.Clamp(value, 1, MaxComboCount);
            if (coercedValue == _minComboCount) return;
            _minComboCount = coercedValue;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    
    private int _maxComboCount = 100;
    public int MaxComboCount
    {
        get => _maxComboCount;
        set
        {
            var coercedValue = Math.Max(value, MinComboCount);
            if (coercedValue == _maxComboCount) return;
            _maxComboCount = coercedValue;
            OnPropertyChanged();
            CreateScenario();
        }
    }
    

    private bool _shouldCheckWakingUp = true;
    public bool ShouldCheckWakingUp
    {
        get => _shouldCheckWakingUp;
        set
        {
            if (value == _shouldCheckWakingUp) return;
            _shouldCheckWakingUp = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }
    
    
    private bool _shouldCheckWallSplat;
    public bool ShouldCheckWallSplat
    {
        get => _shouldCheckWallSplat;
        set
        {
            if (value == _shouldCheckWallSplat) return;
            _shouldCheckWallSplat = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    private bool _shouldCheckAirTech;
    public bool ShouldCheckAirTech
    {
        get => _shouldCheckAirTech;
        set
        {
            if (value == _shouldCheckAirTech) return;
            _shouldCheckAirTech = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    private bool _shouldCheckStartBlocking;
    public bool ShouldCheckStartBlocking
    {
        get => _shouldCheckStartBlocking;
        set
        {
            if (value == _shouldCheckStartBlocking) return;
            _shouldCheckStartBlocking = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    public IScenarioEvent? ScenarioEvent
    {
        get => (IScenarioEvent)GetValue(ScenarioEventProperty);
        set => SetValue(ScenarioEventProperty, value);
    }
    
    public static readonly DependencyProperty ScenarioEventProperty = DependencyProperty.Register(nameof(ScenarioEvent),
        typeof(IScenarioEvent), typeof(EventControl), new PropertyMetadata(default(IScenarioEvent)));

    
    
    private void CreateScenario()
    {
        ScenarioEvent = _selectedScenarioEvent switch
        {
            ScenarioEventTypes.Animation => new AnimationEvent
            {
                ShouldCheckAirTech = ShouldCheckAirTech,
                ShouldCheckStartBlocking = ShouldCheckStartBlocking,
                ShouldCheckWakingUp = ShouldCheckWakingUp,
                ShouldCheckWallSplat = ShouldCheckWallSplat
            },
            ScenarioEventTypes.Combo => new ComboEvent
            {
                MaxComboCount = MaxComboCount,
                MinComboCount = MinComboCount
            },
            _ => null
            
        };
    }
    
    
}

public class EventControlDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate ComboDataTemplate { get; set; } = null!;
    public DataTemplate AnimationDataTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ScenarioEventTypes actionType)
        {
            return actionType switch
            {
                ScenarioEventTypes.Animation => AnimationDataTemplate,
                ScenarioEventTypes.Combo => ComboDataTemplate,
                _ => new DataTemplate()
            };
        }

        return new DataTemplate();
    }
}