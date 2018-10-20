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
using System.Windows.Media.Animation;

namespace GameInterface.GameSettings
{
    /// <summary>
    /// WaitForAIDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class WaitForAIDialog : Window
    {

        internal new Server DataContext { get; set; }
        private SettingStructure SettingStruct { get; set; }

        public WaitForAIDialog()
        {
            InitializeComponent();
        }

        internal WaitForAIDialog(Server serverData, SettingStructure settings)
        {
            this.DataContext = serverData;
            base.DataContext = this.DataContext;
            SettingStruct = settings;
            InitializeComponent();
            serverData.PropertyChanged += __serverDataPropertyChanged;
        }

        private void __serverDataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.StartsWith("IsConnected"))
                return;
            if(Progress1P.IsIndeterminate == true && DataContext.IsConnected1P)
            {
                Progress1P.IsIndeterminate = false;
                Progress1P.Value = 0;
                DoubleAnimation anime = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(1));
                Progress1P.BeginAnimation(ProgressBar.ValueProperty, anime);
            }
            if (Progress2P.IsIndeterminate == true && DataContext.IsConnected2P)
            {
                Progress2P.IsIndeterminate = false;
                Progress2P.Value = 0;
                DoubleAnimation anime = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(1));
                Progress2P.BeginAnimation(ProgressBar.ValueProperty, anime);
            }
            if (((DataContext.IsConnected1P | SettingStruct.IsUser1P) & (DataContext.IsConnected2P | SettingStruct.IsUser2P)))
            {
                DataContext.PropertyChanged -= __serverDataPropertyChanged;
                Dispatcher.BeginInvoke((Action)(() => this.Close()));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if ((SettingStruct.IsUser1P | DataContext.IsConnected1P))
            {
                Text1P.Visibility = ( Progress1P.Visibility =  Visibility.Hidden);
                Progress1P.IsIndeterminate = false;
                Progress1P.Value = 0;
                DoubleAnimation anime = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(1));
                Progress1P.BeginAnimation(ProgressBar.ValueProperty, anime);
            }
            if ((SettingStruct.IsUser2P | DataContext.IsConnected2P))
            {
                Text2P.Visibility = (Progress2P.Visibility = Visibility.Hidden);
                Progress2P.IsIndeterminate = false;
                Progress2P.Value = 0;
                DoubleAnimation anime = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(1));
                Progress2P.BeginAnimation(ProgressBar.ValueProperty, anime);
            }
            if ((Progress1P.IsIndeterminate | Progress2P.IsIndeterminate) == false)
            {
                DataContext.PropertyChanged -= __serverDataPropertyChanged;
                this.Close();
            }
        }
    }
}
