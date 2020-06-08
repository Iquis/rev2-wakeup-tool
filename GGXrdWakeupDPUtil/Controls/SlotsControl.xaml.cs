using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GGXrdWakeupDPUtil.Controls
{
    /// <summary>
    /// Logique d'interaction pour SlotsControls.xaml
    /// </summary>
    public partial class SlotsControl : UserControl
    {
        public SlotsControl()
        {
            InitializeComponent();
        }



        #region SlotNumber Property
        public int SlotNumber
        {
            get { return (int)GetValue(SlotNumberProperty); }
            set { SetValue(SlotNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SlotNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlotNumberProperty =
            DependencyProperty.Register("SlotNumber", typeof(int), typeof(SlotsControl), new FrameworkPropertyMetadata(1, OnSlotNumberPropertyChanged, OnCoerceSlotNumberProperty) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        private static object OnCoerceSlotNumberProperty(DependencyObject source, object baseValue)
        {
            if (baseValue is int value)
            {
                switch (value)
                {
                    case 1:
                    case 2:
                    case 3:
                        return value;

                }
            }

            return SlotNumberProperty.DefaultMetadata.DefaultValue;
        }

        private static void OnSlotNumberPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
        {
            SlotsControl control = source as SlotsControl;

            int value = (int)eventArgs.NewValue;


            switch (value)
            {
                case 1:
                    control.RadioButton1.IsChecked = true;
                    break;
                case 2:
                    control.RadioButton2.IsChecked = true;
                    break;
                case 3:
                    control.RadioButton3.IsChecked = true;
                    break;
            }
        }
        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            this.SlotNumber = 1;
        }

        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            this.SlotNumber = 2;
        }

        private void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            this.SlotNumber = 3;
        }
        #endregion

        #region GroupName Property


        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(SlotsControl), new PropertyMetadata("GroupName"));


        #endregion

    }
}
