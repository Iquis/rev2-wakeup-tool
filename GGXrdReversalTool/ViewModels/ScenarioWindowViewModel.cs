using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Logging;
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

[Obsolete]
public class ScenarioWindowViewModel : ViewModelBase
{
    private readonly IMemoryReader _memoryReader;
    private Scenario? _scenario;
    private readonly UpdateManager _updateManager = new();
    private readonly StringBuilder _logStringBuilder = new();
    public ScenarioWindowViewModel()
    {
        var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();

#if !DEBUG
        if (process == null)
        {
            string message =
                "Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.";
            LogManager.Instance.WriteLine(message);
            MessageBox.Show(message);
            Application.Current.Shutdown();
        }
#endif
        
        LogManager.Instance.MessageDequeued += InstanceOnMessageDequeued;

        _memoryReader = new MemoryReader(process!);

        _scenarioEvents = new ObservableCollection<IScenarioEvent>(new IScenarioEvent[]
        {
            new ComboEvent(),
            new AnimationEvent()
        });


        _scenarioAction = new PlayReversalAction { Input = new SlotInput() };
        _scenarioFrequency = new PercentageFrequency();

    }

    private void InstanceOnMessageDequeued(object? sender, string e)
    {
        _logStringBuilder.AppendLine(e);
        OnPropertyChanged(nameof(LogText));
    }

    #region Window

    #region WindowLoadedCommand
    public RelayCommand WindowLoadedCommand =>  new(Init);
    private void Init()
    {
#if !DEBUG
            if (AutoUpdate)
            {
                Application.Current.MainWindow?.Dispatcher.Invoke(() =>
                {
                    UpdateProcess();
                });
                
            } 
#endif
    }
    #endregion
    
    #region WindowClosedCommand

    public RelayCommand WindowClosedCommand => new(Dispose);
    private void Dispose()
    {
        _scenario?.Stop();
    }
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

        var config = ReversalToolConfiguration.GetConfig();
        config.ReplayTriggerType = parameter;
        ReversalToolConfiguration.SaveConfig(config);
        
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
        UpdateProcess(true);
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

    public RelayCommand<Window> AboutCommand => new(About);

    private void About(Window mainWindow)
    {
        var aboutWindow = new AboutWindow
        {
            Owner = mainWindow
        };
        
        aboutWindow.ShowDialog();
    }
    #endregion
    
    public string Title => $"GGXrd Rev 2 Reversal Tool v{ReversalToolConfiguration.Get("CurrentVersion")}";

    public bool AutoUpdate
    {
        get
        {
            var config = ReversalToolConfiguration.GetConfig();
            return config.AutoUpdate;
        }
        set
        {
            var config = ReversalToolConfiguration.GetConfig();
            config.AutoUpdate = value;
            ReversalToolConfiguration.SaveConfig(config);
            

            OnPropertyChanged();
        }
    }

    // public IEnumerable<Preset> Presets
    // {
    //     get
    //     {
    //         var result = Preset.Presets;
    //         return result;
    //     }
    //     set
    //     {
    //         
    //     }
    // }

    public IEnumerable<Preset> Presets
    {
        get => _presets;
        set
        {
            if (Equals(value, _presets)) return;
            _presets = value;
            OnPropertyChanged();
        }
    }


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
        private set
        {
            // if (Equals(value, _selectedScenarioAction)) return;
            _scenarioAction = value;
            OnPropertyChanged();
        }
    }

    private readonly IScenarioFrequency? _scenarioFrequency;
    public IScenarioFrequency? ScenarioFrequency => _scenarioFrequency;


    #region EnableCommand

    public RelayCommand EnableCommand => new(Enable, CanEnable);

    private void Enable()
    {
        if (SelectedScenarioEvent == null || ScenarioAction == null || ScenarioFrequency == null)
        {
            return;
        }

        ScenarioAction.SlotNumber = SlotNumber;

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
            var newInput = playReversal.Input.RawInputText +
                           $"{(!playReversal.Input.RawInputText.EndsWith(",") && !string.IsNullOrWhiteSpace(playReversal.Input.RawInputText)  ? "," : "")}" +
                           input;

            ScenarioAction = new PlayReversalAction()
                { Input = new SlotInput(newInput), MemoryReader = playReversal.MemoryReader };
        }
    }

    private bool CanInsertPresetInput(string input)
    {
        return _scenario is not { IsRunning: true };
    }

    #endregion

    
    public string LogText => _logStringBuilder.ToString();

    private int _slotNumber = 1;
    private IEnumerable<Preset> _presets;


    public int SlotNumber
    {
        get => _slotNumber;
        set
        {
            if (value == _slotNumber) return;
            _slotNumber = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable Options => new []{ 1,2,3};

    public int SelectedOption { get; set; } = 1;

    private void UpdateProcess(bool confirm = false)
    {
        try
        {
                _updateManager.CleanOldFiles();
                var latestVersion = _updateManager.CheckUpdates();

                var config = ReversalToolConfiguration.GetConfig();
                var currentVersion = config.CurrentVersion;
                
                LogManager.Instance.WriteLine($"Current Version is {currentVersion}");
                
                
                switch (Math.Sign(currentVersion.CompareTo(latestVersion.Version)))
                {
                    case 0:
                        LogManager.Instance.WriteLine("No updates");
                        if (confirm)
                        {
                            MessageBox.Show($"Your version is up to date.\r\nYour version : \t {currentVersion}");
                        }
                
                        break;
                    case -1:
                        if (!confirm ||
                            MessageBox.Show(
                                $"A new version is available ({latestVersion.Version})\r\bDo you want do download it?",
                                "New version available", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            LogManager.Instance.WriteLine($"Found new version : v{latestVersion.Version}");
                            bool downloadSuccess = _updateManager.DownloadUpdate(latestVersion);
                
                            if (downloadSuccess)
                            {
                                bool installSuccess = _updateManager.InstallUpdate();
                
                                if (installSuccess)
                                {
                                    _updateManager.SaveVersion(latestVersion);
                                    _updateManager.RestartApplication();
                                }
                            }
                        }
                
                        break;
                    case 1:
                        LogManager.Instance.WriteLine("No updates");
                        if (confirm)
                        {
                            MessageBox.Show(
                                $"You got a newer version.\r\nYour version :\t{currentVersion}\r\nAvailable version :\t{latestVersion.Version}");
                        }
                
                        break;
                }
        }
        catch (Exception ex)
        {
            LogManager.Instance.WriteException(ex);
        }
    }
}