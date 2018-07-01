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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interface
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new
            {
                管理者 = new[] { new { 姓 = "岩永", 名 = "信之" } },
                コンテンツ = new[]
                {
                    new { タイトル = "C# 入門", URL = "csharp" },
                    new { タイトル = "信号処理", URL = "dsp" },
                    new { タイトル = "力学", URL = "dynamics" },
                }
            };
        }
    }
}
