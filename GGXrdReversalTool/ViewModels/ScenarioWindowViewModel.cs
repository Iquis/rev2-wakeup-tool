﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Memory.Implementations;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Presets;
using GGXrdReversalTool.Library.Scenarios;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Action.Implementations;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using GGXrdReversalTool.Library.Scenarios.Frequency.Implementations;
using GGXrdReversalTool.Updates;

namespace GGXrdReversalTool.ViewModels;

public class ScenarioWindowViewModel : ViewModelBase
{
    private readonly IMemoryReader _memoryReader;
    private Scenario? _scenario;
    private UpdateManager _updateManager = new();
    public ScenarioWindowViewModel()
    {
        var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();

#if !DEBUG
        if (process == null)
        {
            //TODO explicit error
            throw new NotImplementedException();
        }
#endif

        _memoryReader = new MemoryReader(process);

        _scenarioEvents = new ObservableCollection<IScenarioEvent>(new IScenarioEvent[]
        {
            new ComboEvent(),
            new AnimationEvent()
        });


        _scenarioAction = new PlayReversalAction() { Input = new SlotInput() };
        _scenarioFrequency = new PercentageFrequency();

    }

    #region Window

    #region WindowLoadedCommand
    public RelayCommand WindowLoadedCommand =>  new(Init);
    #endregion
    
    #region WindowClosedCommand

    public RelayCommand WindowClosedCommand => new(Dispose);
    #endregion

    #endregion
    
    #region ReplayTrigger

    public bool IsAsmReplayTypeChecked => ReversalToolConfiguration.Get("ReplayTriggerType") == ReplayTriggerTypes.AsmInjection.ToString();
    public bool IsKeyStrokeReplayTypeChecked => ReversalToolConfiguration.Get("ReplayTriggerType") == ReplayTriggerTypes.Keystroke.ToString();



    public RelayCommand<string> ChangeReplayTypeCommand => new(ChangeReplayTypeCommandExecute, ChangeReplayTypeCommandCanExecute);
    private void ChangeReplayTypeCommandExecute(string parameter)
    {
        if (!Enum.TryParse(parameter, false, out ReplayTriggerTypes value))
        {
            return;
        }

        ReversalToolConfiguration.Set("ReplayTriggerType", parameter);
        _scenarioAction?.Init();

        OnPropertyChanged(nameof(IsAsmReplayTypeChecked));
        OnPropertyChanged(nameof(IsKeyStrokeReplayTypeChecked));
    }
    private bool ChangeReplayTypeCommandCanExecute(string parameter)
    {
        return ReversalToolConfiguration.Get("ReplayTriggerType") != parameter;
    }

        

    #endregion
    
    #region CheckUpdatesCommand

    public RelayCommand CheckUpdatesCommand =>  new(CheckUpdates, CanCheckUpdates);

    private bool CanCheckUpdates()
    {
        return _scenario is not { IsRunning: true };
    }
    private void CheckUpdates()
    {
        _updateManager.UpdateApplication();
    }



    #endregion

    #region DonateCommand

    public RelayCommand DonateCommand => new(Donate);

    private void Donate()
    {
        string target = "https://paypal.me/Iquisiquis";
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });

    }

    #endregion

    #region AboutCommand

    public RelayCommand AboutCommand => new(About);

    private void About()
    {
        throw new NotImplementedException();
    }
    #endregion
    
    public string Title => $"GGXrd Rev 2 Reversal Tool v{ReversalToolConfiguration.Get("CurrentVersion")}";

    public bool AutoUpdate
    {
        get => ReversalToolConfiguration.Get("AutoUpdate") == "True";
        set
        {
            ReversalToolConfiguration.Set("AutoUpdate", value ? "1" : "0");

            OnPropertyChanged();
        }
    }

    public IEnumerable<Preset> Presets => Preset.Presets;

    private ObservableCollection<IScenarioEvent> _scenarioEvents;
    public ObservableCollection<IScenarioEvent> ScenarioEvents
    {
        get => _scenarioEvents;
        set
        {
            if (Equals(value, _scenarioEvents)) return;
            _scenarioEvents = value;
            OnPropertyChanged();
        }
    }

    private IScenarioEvent? _selectedScenarioEvent;
    public IScenarioEvent? SelectedScenarioEvent
    {
        get => _selectedScenarioEvent;
        set
        {
            if (Equals(value, _selectedScenarioEvent)) return;
            _selectedScenarioEvent = value;
            OnPropertyChanged();
        }
    }


    private IScenarioAction? _scenarioAction;
    public IScenarioAction? ScenarioAction
    {
        get => _scenarioAction;
        set
        {
            // if (Equals(value, _selectedScenarioAction)) return;
            _scenarioAction = value;
            OnPropertyChanged();
        }
    }

    private IScenarioFrequency? _scenarioFrequency;
    public IScenarioFrequency? ScenarioFrequency
    {
        get => _scenarioFrequency;
        set
        {
            if (Equals(value, _scenarioFrequency)) return;
            _scenarioFrequency = value;
            OnPropertyChanged();
        }
    }


    #region EnableCommand

    public RelayCommand EnableCommand => new(Enable, CanEnable);

    private void Enable()
    {
        if (SelectedScenarioEvent == null || ScenarioAction == null || ScenarioFrequency == null)
        {
            return;
        }

        _scenario = new Scenario(_memoryReader, SelectedScenarioEvent, ScenarioAction, ScenarioFrequency);
        
        _scenario.Run();
    }

    private bool CanEnable()
    {
        
        return _selectedScenarioEvent != null &&
               _scenarioAction != null &&
               _scenarioFrequency != null &&
               
               //TODO check with all event types
               ((_selectedScenarioEvent is AnimationEvent && _scenarioAction.Input.IsReversalValid) || (_selectedScenarioEvent is ComboEvent && _scenarioAction.Input.IsValid)) &&
               _scenario is not { IsRunning: true };
    }

    #endregion

    #region DisableCommand

    public RelayCommand DisableCommand => new(Disable, CanDisable);

    private void Disable()
    {
        _scenario?.Stop();
    }

    private bool CanDisable()
    {
        return _scenario?.IsRunning ?? false;
        
        
        //TODO Implement
    }

    #endregion

    #region InsertPresetInputCommand

    public RelayCommand<string> InsertPresetInputCommand => new(InsertPresetInput, CanInsertPresetInput);

    private void InsertPresetInput(string input)
    {
        if (_scenarioAction is PlayReversalAction playReversal)
        {
            var newInput = playReversal.Input.InputText +
                           $"{(!playReversal.Input.InputText.EndsWith(",") && !string.IsNullOrWhiteSpace(playReversal.Input.InputText)  ? "," : "")}" +
                           input;

            ScenarioAction = new PlayReversalAction()
                { Input = new SlotInput(newInput), MemoryReader = playReversal.MemoryReader };
        }
    }

    private bool CanInsertPresetInput(string input)
    {
        //TODO implement (cannot insert if scenario is running)
        return true;
    }

    #endregion

    private void Init()
    {
        // var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();
        // var memoryReader = new MemoryReader(process);
        // // var scenarioEvent = new WakeupEvent();
        // // var scenarioEvent = new ComboEvent();
        // var scenarioEvent = new AnimationEvent();
        // var scenarioAction = new PlayReversalAction();
        // var scenarioFrequency = new PercentageFrequency(100);
        //
        // var scenario = new Scenario(memoryReader, scenarioEvent, scenarioAction, scenarioFrequency);
        //
        // scenario.Run();
    }

    private void Dispose()
    {
        _scenario?.Stop();
    }
}