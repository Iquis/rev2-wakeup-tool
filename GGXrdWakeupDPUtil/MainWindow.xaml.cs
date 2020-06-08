using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GGXrdWakeupDPUtil.ViewModels;

namespace GGXrdWakeupDPUtil
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindowViewModel viewModel = this.DataContext as MainWindowViewModel;

            var command = viewModel.WindowClosedCommand;

            if (command.CanExecute())
            {
                command.Execute();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = this.DataContext as MainWindowViewModel;

            var command = viewModel.WindowLoadedCommand;

            if (command.CanExecute())
            {
                command.Execute();
            }
        }
    }
}
