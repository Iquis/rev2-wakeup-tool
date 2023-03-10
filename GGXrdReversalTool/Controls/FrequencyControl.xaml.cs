using System;
using System.Windows;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using GGXrdReversalTool.Library.Scenarios.Frequency.Implementations;

namespace GGXrdReversalTool.Controls;

public sealed partial class FrequencyControl : NotifiedUserControl
{
    public FrequencyControl()
    {
        InitializeComponent();
    }

    private int _percentage = 100;
    public int Percentage
    {
        get => _percentage;
        set
        {
            var coercedValue = Math.Clamp(value, 0, 100);
            if (coercedValue == _percentage) return;
            _percentage = coercedValue;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    public IScenarioFrequency? ScenarioFrequency
    {
        get => (IScenarioFrequency)GetValue(ScenarioFrequencyProperty);
        set => SetValue(ScenarioFrequencyProperty, value);
    }

    public static readonly DependencyProperty ScenarioFrequencyProperty =
        DependencyProperty.Register(nameof(ScenarioFrequency), typeof(IScenarioFrequency), typeof(FrequencyControl),
            new PropertyMetadata(new PercentageFrequency { Percentage = 100 }));

    private void CreateScenario()
    {
        ScenarioFrequency = new PercentageFrequency()
        {
            Percentage = Percentage
        };
    }
}