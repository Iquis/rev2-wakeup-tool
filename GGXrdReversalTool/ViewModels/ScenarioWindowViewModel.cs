using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Memory.Implementations;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using GGXrdReversalTool.Updates;
using Microsoft.Win32;

namespace GGXrdReversalTool.ViewModels;

public class ScenarioWindowViewModel : ViewModelBase
{
    private readonly UpdateManager _updateManager = new();
    private readonly IMemoryReader _memoryReader = null!;
    private readonly StringBuilder _logStringBuilder = new();
    private Scenario? _scenario;
    public IScenarioEvent? ScenarioEvent { get; set; }
    public IScenarioAction? ScenarioAction { get; set; }
    public IScenarioFrequency? ScenarioFrequency { get; set; }

    public ScenarioWindowViewModel()
    {
        var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();
        
        if (process == null)
        {
            var aboutWindow = new AboutWindow(offlineMode: true);
        
            aboutWindow.ShowDialog();
            
            Application.Current.Shutdown();
            return;
        }

        LogManager.Instance.MessageDequeued += InstanceOnMessageDequeued;
        
        //TODO injection
        _memoryReader = new MemoryReader(process);


    }

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

    public RelayCommand<string> DonateCommand => new(Donate);

    private void Donate(string target)
    {
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

    #region ImportCommand

    public RelayCommand<int> ImportCommand => new(Import, CanImport);

    private void Import(int slotNumber)
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

        if (slotInput.IsValid && _memoryReader.WriteInputInSlot(slotNumber, slotInput))
        {
            MessageBox.Show("Inputs has been inserted in slot : " + slotNumber, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Failed to import inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanImport(int slotNumber)
    {
        return !IsRunning;
    }

    #endregion

    #region ExportCommand

    public RelayCommand<int> ExportCommand => new(Export);

    private void Export(int slotNumber)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
        };

        var dialogResult = saveFileDialog.ShowDialog();

        if (!dialogResult.HasValue || !dialogResult.Value) return;

        var slotInput = _memoryReader.ReadInputFromSlot(slotNumber);

        if (slotInput.IsValid)
        {
            try
            {
                using var streamWriter = new StreamWriter(saveFileDialog.FileName);
                
                streamWriter.Write(slotInput.CondensedInputText);
                
                MessageBox.Show("Inputs Exported!");
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e);
                
                MessageBox.Show("Failed to export inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Inputs are invalid!");
        }


    }


    #endregion
}