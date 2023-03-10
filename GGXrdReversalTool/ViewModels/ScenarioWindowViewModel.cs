using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Memory.Implementations;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Scenarios;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using GGXrdReversalTool.Updates;

namespace GGXrdReversalTool.ViewModels;

public class ScenarioWindowViewModel : ViewModelBase
{
    private readonly UpdateManager _updateManager = new();
    private readonly IMemoryReader _memoryReader;
    private readonly StringBuilder _logStringBuilder = new();
    private Scenario? _scenario;
    public IScenarioEvent? ScenarioEvent { get; set; }
    public IScenarioAction? ScenarioAction { get; set; }
    public IScenarioFrequency? ScenarioFrequency { get; set; }

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
        
        
    }

    public string Title => $"GGXrd Rev 2 Reversal Tool v{ReversalToolConfiguration.Get("CurrentVersion")}";
    
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

        OnPropertyChanged(nameof(IsAsmReplayTypeChecked));
        OnPropertyChanged(nameof(IsKeyStrokeReplayTypeChecked));
    }
    private bool ChangeReplayTypeCommandCanExecute(string parameter)
    {
        return ReversalToolConfiguration.Get("ReplayTriggerType") != parameter && _scenario is not { IsRunning: true };
    }

        

    #endregion
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
    
    #region DonateCommand

    public RelayCommand DonateCommand => new(Donate);

    private void Donate()
    {
        string target = "https://paypal.me/Iquisiquis";
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });

    }

    #endregion

    public bool IsRunning => _scenario is { IsRunning: true };

    private void InstanceOnMessageDequeued(object? sender, string e)
    {
        _logStringBuilder.AppendLine(e);
        OnPropertyChanged(nameof(LogText));
    }
    
    public string LogText => _logStringBuilder.ToString();

    private int _slotNumber = 1;
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

    #region EnableCommand

    public RelayCommand EnableCommand => new(Enable, CanEnable);

    private void Enable()
    {
        ScenarioAction!.SlotNumber = SlotNumber;
        _scenario = new Scenario(_memoryReader, ScenarioEvent!, ScenarioAction, ScenarioFrequency!);
        
        _scenario.Run();
        
        OnPropertyChanged(nameof(IsRunning));
    }

    private bool CanEnable()
    {
        if (_scenario is {IsRunning: true})
        {
            return false;
        }

        if (ScenarioEvent is null)
        {
            return false;
        }

        if (ScenarioAction is null)
        {
            return false;
        }

        if (ScenarioFrequency is null)
        {
            return false;
        }
        
        switch (ScenarioEvent)
        {
            case ComboEvent when ScenarioAction.Input.IsValid:
            case AnimationEvent when ScenarioAction.Input.IsReversalValid && ScenarioEvent.IsValid:
                return true;
            default:
                return false;
        }
    }

    #endregion

    #region DisableCommand

    public RelayCommand DisableCommand => new(Disable, CanDisable);

    private void Disable()
    {
        _scenario?.Stop();
        
        OnPropertyChanged(nameof(IsRunning));
    }

    private bool CanDisable()
    {
        return _scenario is { IsRunning: true };
    }

    #endregion
}