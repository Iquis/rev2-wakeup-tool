using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Models;

namespace GGXrdReversalTool.ViewModels;

public class AboutViewModel : ViewModelBase
{

    public string Title => $"GGXrd Rev 2 Reversal Tool v{ReversalToolConfiguration.Get("CurrentVersion")}";

    public IEnumerable<ContributorGroupViewModel> ContributorList =>
        Contributors.AppContributors.GroupBy(x => x.Role)
            .Select(group => new ContributorGroupViewModel
            {
                Name = group.Key,
                Contributors = group
            });

    #region NavigateCommand

    public RelayCommand<string> NavigateCommand => new(Navigate);

    private void Navigate(string parameter)
    {
        Process.Start(new ProcessStartInfo(parameter) { UseShellExecute = true });
    }

    #endregion

    #region MailToCommand

    public RelayCommand MailToCommand => new(MailTo);

    private void MailTo()
    {
        string target = "mailto:iquis01@gmail.com";
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }

    #endregion
    
    #region FacebookCommand

    public RelayCommand FacebookCommand => new(NavigateFacebook);

    private void NavigateFacebook()
    {
        string target = "https://www.facebook.com/iquisfgc";
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }

    #endregion

    #region Twitter Command

    public RelayCommand TwitterCommand => new(TwitterNavigate);

    private bool _offlineMode;
    public bool OfflineMode
    {
        get => _offlineMode;
        set
        {
            if (value == _offlineMode) return;
            _offlineMode = value;
            OnPropertyChanged();
        }
    }

    private void TwitterNavigate()
    {
        string target = "https://twitter.com/iquisiquis";
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }

    #endregion

    #region DonationCommand

    public RelayCommand<string> DonateCommand => new(Donate);

    private void Donate(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    #endregion

    public IEnumerable<DonationLink> DonationLinks => Models.DonationLinks.Items;
}