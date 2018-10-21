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
    /// ReconnectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ReconnectDialog : Window
    {
        internal new Server DataContext { get; set; }
        private int  PlayerNum { get; set; }

        public ReconnectDialog()
        {
            InitializeComponent();
        }

        internal ReconnectDialog(Server serverData, int playerNum)
        {
            this.DataContext = serverData;
            base.DataContext = this.DataContext;
            PlayerNum = playerNum;
            InitializeComponent();
            PlayerText.Text = (playerNum + 1).ToString() + "P";
            serverData.PropertyChanged += __serverDataPropertyChanged;
            if (playerNum == 0 ? DataContext.IsConnected1P : DataContext.IsConnected2P)
            {
                DataContext.PropertyChanged -= __serverDataPropertyChanged;
                this.Close();
            }
        }

        private void __serverDataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.StartsWith("IsConnected"))
                return;
            if (PlayerNum == 0 ? DataContext.IsConnected1P : DataContext.IsConnected2P)
            {
                DataContext.PropertyChanged -= __serverDataPropertyChanged;
                Dispatcher.BeginInvoke((Action)(() => this.Close()));
            }
        }
    }
}
