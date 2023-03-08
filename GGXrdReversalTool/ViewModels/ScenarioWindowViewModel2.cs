using System;
using System.Diagnostics;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Updates;

namespace GGXrdReversalTool.ViewModels;

//TODO Rename
public class ScenarioWindowViewModel2 : ViewModelBase
{
    private readonly UpdateManager _updateManager = new();

    public ScenarioWindowViewModel2()
    {
        
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
        
        //TODO Implement
        //_scenarioAction?.Init();

        OnPropertyChanged(nameof(IsAsmReplayTypeChecked));
        OnPropertyChanged(nameof(IsKeyStrokeReplayTypeChecked));
    }
    private bool ChangeReplayTypeCommandCanExecute(string parameter)
    {
        return ReversalToolConfiguration.Get("ReplayTriggerType") != parameter;
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
        return true;
        //TODO Implement
        // return _scenario is not { IsRunning: true };
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
}