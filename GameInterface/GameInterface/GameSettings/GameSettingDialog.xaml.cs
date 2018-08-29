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
    /// GameSettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GameSettingDialog : Window
    {
        new SettingStructure DataContext { get; set; }
        bool IsCanceled { get; set; } = true;

        Random randomer = new Random();

        public GameSettingDialog()
        {
            DataContext = new SettingStructure();
            base.DataContext = DataContext;
            InitializeComponent();
        }

        internal static bool ShowDialog(out SettingStructure setting)
        {
            GameSettingDialog dialog = new GameSettingDialog();
            dialog.ShowDialog();
            if (dialog.IsCanceled)
            {
                setting = null;
                return false;
            }
            else
            {
                setting = dialog.DataContext;
                return true;
            }
        }

        private void P1UserToggle_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.Port1P = 0;
        }

        private void P1AIToggle_Checked(object sender, RoutedEventArgs e)
        {
            Set1PPort();
        }

        private void Set1PPort()
        {
            if (!this.IsInitialized) return;
            P1PortBox.Background = new SolidColorBrush(Colors.White);
            P1PortBoxErrorMessage.Text = "";
            P1PortBoxErrorMessage.Visibility = Visibility.Collapsed;
            if(ushort.TryParse(P1PortBox.Text, out var number))
            {
                if (number <= 9999)
                    ErrorSet(P1PortBox, P1PortBoxErrorMessage, "ポート番号は10000以上でなければなりません.");
                else if(number == DataContext.Port2P)
                    ErrorSet(P1PortBox, P1PortBoxErrorMessage, "2Pのポート番号と異なる番号を指定する必要があります.");
                DataContext.Port1P = number;
            }
            else
                ErrorSet(P1PortBox, P1PortBoxErrorMessage, "10000～65535の整数を入力してください．");
        }

        private void P2UserToggle_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.Port2P = 0;
        }

        private void P2AIToggle_Checked(object sender, RoutedEventArgs e)
        {
            Set2PPort();
        }

        private void Set2PPort()
        {
            if (!this.IsInitialized) return;
            P2PortBox.Background = new SolidColorBrush(Colors.White);
            P2PortBoxErrorMessage.Text = "";
            P2PortBoxErrorMessage.Visibility = Visibility.Collapsed;
            if (ushort.TryParse(P2PortBox.Text, out var number))
            {
                if (number <= 9999)
                    ErrorSet(P2PortBox, P2PortBoxErrorMessage, "ポート番号は10000以上でなければなりません.");
                else if (number == DataContext.Port1P)
                    ErrorSet(P2PortBox, P2PortBoxErrorMessage, "1Pのポート番号と異なる番号を指定する必要があります.");
                DataContext.Port2P = number;
            }
            else
                ErrorSet(P2PortBox, P2PortBoxErrorMessage, "10000～65535の整数を入力してください．");
        }

        private void ErrorSet(TextBox portBox, TextBlock messageBlock, string message)
        {
            portBox.Background = new SolidColorBrush(Colors.LightPink);
            messageBlock.Text = message;
            messageBlock.Visibility = Visibility.Visible;
        }

        private void P1PortBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Set1PPort();
        }

        private void P2PortBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Set2PPort();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = false;
            Close();
        }

        private void WidthBox_RandomButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext.BoardWidth = (byte)randomer.Next(2, 13);
        }

        private void HeightBox_RandomButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext.BoardHeight = (byte)randomer.Next(2, 13);

        }
    }
}
