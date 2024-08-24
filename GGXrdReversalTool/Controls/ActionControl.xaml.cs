using System;
using System.IO;
using System.Linq;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Action.Implementations;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;
using Microsoft.Win32;

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
    
    private Visibility _tooShortInfoVisible = Visibility.Collapsed;
    public Visibility TooShortInfoVisible
    {
        get => _tooShortInfoVisible;
        set
        {
            if (value == _tooShortInfoVisible) return;
            _tooShortInfoVisible = value;
            OnPropertyChanged();
        }
    }
    
    private Visibility _noStartMarkerWarningVisible = Visibility.Collapsed;
    public Visibility NoStartMarkerWarningVisible
    {
        get => _noStartMarkerWarningVisible;
        set
        {
            if (value == _noStartMarkerWarningVisible) return;
            _noStartMarkerWarningVisible = value;
            OnPropertyChanged();
        }
    }
    
    private IScenarioEvent? _scenarioEvent;
    public IScenarioEvent? ScenarioEvent
    {
        get => (IScenarioEvent)GetValue(ScenarioEventProperty);
        set
        {
            if (_scenarioEvent == value) return;
            _scenarioEvent = value;
            UpdateWarnings();
        }
    }

    public static readonly DependencyProperty ScenarioEventProperty =
        DependencyProperty.Register(nameof(ScenarioEvent), typeof(IScenarioEvent), typeof(ActionControl),
            new FrameworkPropertyMetadata(null, OnScenarioEventPropertyChanged)
            {
                BindsTwoWayByDefault = false
            });

    private static void OnScenarioEventPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs eventArgs)
    {
        if (source is not ActionControl control)
        {
            return;
        }
        
        var value = eventArgs.NewValue;
        control.ScenarioEvent = (IScenarioEvent)value;
    }

    public RelayCommand ImportCommand => new(Import);
    private void Import()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
        };
        var dialogResult = openFileDialog.ShowDialog();

        if (!dialogResult.HasValue || !dialogResult.Value) return;
        
        using var streamReader = new StreamReader(openFileDialog.FileName);

        var content = streamReader.ReadToEnd();

        var slotInput = new SlotInput(content);


        if (slotInput.IsValid)
        {
            RawInputText = slotInput.CondensedInputListText.Aggregate((a, b) => $"{a},{b}");
        }
        else
        {
            MessageBox.Show("Failed to import inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public RelayCommand ExportCommand => new(Export, CanExport);

    private void Export()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
        };

        var dialogResult = saveFileDialog.ShowDialog();

        if (!dialogResult.HasValue || !dialogResult.Value) return;

        var condensedInputText = new SlotInput(RawInputText).CondensedInputListText.Aggregate((a, b) => $"{a},{b}");

        try
        {
            using var streamWriter = new StreamWriter(saveFileDialog.FileName);
            streamWriter.Write(condensedInputText);

            MessageBox.Show("Inputs Exported!");
        }
        catch (Exception e)
        {
            LogManager.Instance.WriteException(e);

            MessageBox.Show("Failed to export inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private bool CanExport() => ScenarioAction is { Input.IsValid: true };

    #region InsertPresetInputCommand

    public RelayCommand<string> InsertPresetInputCommand => new(InsertPresetInput, CanInsertPresetInput);

    

    private void InsertPresetInput(string input)
    {
        RawInputText = RawInputText +
                       $"{(!RawInputText.EndsWith(",") && !string.IsNullOrWhiteSpace(RawInputText)  ? "," : "")}" +
                       input;
    }

    private bool CanInsertPresetInput(string input) => IsEnabled;

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
        UpdateWarnings();
    }
    
    private void UpdateWarnings()
    {
        UpdateTooShort();
        UpdateMissingStart();
    }
    
    private void UpdateTooShort()
    {
        if (ScenarioAction == null || RawInputText.Length == 0) {
            TooShortInfoVisible = Visibility.Collapsed;
            return;
        }
        int tooShortLength = 4;
        int count = 0;
        foreach (string input in ScenarioAction.Input.ExpandedInputList) {
            ++count;
            if (count > tooShortLength) break;
        }
        if (count != 0 && count <= tooShortLength) {
            TooShortInfoVisible = Visibility.Visible;
        } else {
            TooShortInfoVisible = Visibility.Collapsed;
        }
    }
    
    private void UpdateMissingStart()
    {
        if (ScenarioEvent != null && ScenarioEvent is ComboEvent || RawInputText.Length == 0) {
            NoStartMarkerWarningVisible = Visibility.Collapsed;
            return;
        }

        NoStartMarkerWarningVisible = RawInputText.Contains('!') ? Visibility.Collapsed : Visibility.Visible;
    }

}