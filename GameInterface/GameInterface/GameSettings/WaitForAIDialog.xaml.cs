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
            if (settings.IsUser1P)
                Text1P.Visibility = ( Progress1P.Visibility =  Visibility.Hidden);
            if (settings.IsUser2P)
                Text2P.Visibility = (Progress2P.Visibility = Visibility.Hidden);
        }

        private void __serverDataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("IsConnected") && ((DataContext.IsConnected1P | SettingStruct.IsUser1P) & (DataContext.IsConnected2P | SettingStruct.IsUser2P)))
            {
                DataContext.PropertyChanged -= __serverDataPropertyChanged;
                Dispatcher.BeginInvoke((Action)(() => this.Close()));
            }
        }
    }
}
