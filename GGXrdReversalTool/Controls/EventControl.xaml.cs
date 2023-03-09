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

    // public IEnumerable<ScenarioActionTypes> ActionTypes { get; set; }

    public IEnumerable<ScenarioActionTypes> ActionTypes
    {
        get => (IEnumerable<ScenarioActionTypes>)GetValue(ActionTypesProperty);
        set => SetValue(ActionTypesProperty, value);
    }

    public static readonly DependencyProperty ActionTypesProperty = DependencyProperty.Register(
        nameof(ActionTypes), typeof(IEnumerable<ScenarioActionTypes>), typeof(EventControl), new PropertyMetadata(default(IEnumerable<ScenarioActionTypes>)));

    private ScenarioActionTypes _selectedScenarioEvent;

    public ScenarioActionTypes SelectedScenarioEvent
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

}