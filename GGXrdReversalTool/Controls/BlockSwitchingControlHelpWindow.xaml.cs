using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GGXrdReversalTool.Controls
{
    public partial class BlockSwitchingControlHelpWindow : Window, INotifyPropertyChanged
    {
        public BlockSwitchingControlHelpWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private bool _showMoreInfo = false;
        public bool ShowMoreInfo
        {
            get => _showMoreInfo;
            set
            {
                if (_showMoreInfo == value) return;
                _showMoreInfo = value;
                OnPropertyChanged();
            }
        }
        
        private string _hyperlinkText = "Show some caveats";
        public string HyperlinkText
        {
            get => _hyperlinkText;
            set
            {
                if (_hyperlinkText.Equals(value)) return;
                _hyperlinkText = value;
                OnPropertyChanged();
            }
        }
        
        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            ShowMoreInfo = !_showMoreInfo;
            HyperlinkText = _showMoreInfo ? "Hide caveats" : "Show some caveats";
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
