using System;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Action.Implementations;

namespace GGXrdReversalTool.Controls;

public sealed partial class ActionControl
{
    public ActionControl()
    {
        InitializeComponent();
    }

    private string _rawInputText = string.Empty;
    public string RawInputText
    {
        get => _rawInputText;
        set
        {
            if (value == _rawInputText) return;
            _rawInputText = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }
    

    #region InsertPresetInputCommand

    public RelayCommand<string> InsertPresetInputCommand => new(InsertPresetInput, CanInsertPresetInput);

    

    private void InsertPresetInput(string input)
    {
        RawInputText = RawInputText +
                       $"{(!RawInputText.EndsWith(",") && !string.IsNullOrWhiteSpace(RawInputText)  ? "," : "")}" +
                       input;
    }

    private bool CanInsertPresetInput(string input)
    {
        return IsEnabled;
    }

    #endregion
    
    public IScenarioAction? ScenarioAction
    {
        get => (IScenarioAction?)GetValue(ScenarioActionProperty);
        set => SetValue(ScenarioActionProperty, value);
    }

    public static readonly DependencyProperty ScenarioActionProperty =
        DependencyProperty.Register(nameof(ScenarioAction), typeof(IScenarioAction), typeof(ActionControl),
            new PropertyMetadata(default(IScenarioAction?)));


    private void CreateScenario()
    {
        ScenarioAction = new PlayReversalAction
        {
            Input = new SlotInput(RawInputText)
        };
    }

    
}