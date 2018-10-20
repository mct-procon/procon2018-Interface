using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace GameInterface.QRCodeReader
{
    /// <summary>
    /// EnemyAgentSelectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EnemyAgentSelectDialog : Window
    {
        public new EnemyAgentSelectViewModel DataContext;

        public EnemyAgentSelectDialog(EnemyAgentSelectViewModel viewmodel)
        {
            DataContext = viewmodel;
            base.DataContext = DataContext;
            InitializeComponent();
        }

        public static bool ShowDialog(out AgentPositioningState Result, GameSettings.SettingStructure settingStructure)
        {
            Result = AgentPositioningState.Error;
            var vm = new EnemyAgentSelectViewModel();
            vm.Init(settingStructure.QCCell.GetLength(0), settingStructure.QCCell.GetLength(1), settingStructure.QCAgent);
            var dig = new EnemyAgentSelectDialog(vm);
            if(dig.ShowDialog() == true)
            {
                Result = dig.DataContext.PositionState;
                if (Result == AgentPositioningState.Error)
                    return false;
                return true;
            }
            return false;
        }

        public void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext.PositionState == AgentPositioningState.Error) return;
            this.DialogResult = true;
            Close();
        }
    }

    public class EnemyAgentSelectViewModel : ViewModels.ViewModelBase
    {
        private const int ImageSize = 300;

        private WriteableBitmap horizontalResultBitmap = null;
        public WriteableBitmap HorizontalResultBitmap {
            get => horizontalResultBitmap;
            set => RaisePropertyChanged(ref horizontalResultBitmap, value);
        }

        private WriteableBitmap verticalResultBitmap = null;
        public WriteableBitmap VerticalResultBitmap {
            get => verticalResultBitmap;
            set => RaisePropertyChanged(ref verticalResultBitmap, value);
        }

        private AgentPositioningState positionState = AgentPositioningState.Error;
        public AgentPositioningState PositionState {
            get => positionState;
            set => RaisePropertyChanged(ref positionState, value);
        }


        public void Init(int BoardWidth, int BoardHeight, Agent[] Agents)
        {
            // Horizontal
            var enemy1 = new Point(Agents[0].Point.X, BoardHeight - Agents[0].Point.Y - 1);
            var enemy2 = new Point(Agents[1].Point.X, BoardHeight - Agents[1].Point.Y - 1);
            HorizontalResultBitmap = Draw(BoardWidth, BoardHeight, new[] { Agents[0].Point, Agents[1].Point, enemy1, enemy2 });
            enemy1 = new Point(BoardWidth - Agents[0].Point.X - 1, Agents[0].Point.Y);
            enemy2 = new Point(BoardWidth - Agents[1].Point.X - 1, Agents[1].Point.Y);
            VerticalResultBitmap = Draw(BoardWidth, BoardHeight, new[] { Agents[0].Point, Agents[1].Point, enemy1, enemy2 });
        }

        private WriteableBitmap Draw(int BoardWidth, int BoardHeight, Point[] Agents)
        {
            WriteableBitmap Result = new WriteableBitmap(ImageSize, ImageSize, 96, 96, PixelFormats.Bgra32, null);

            int CellSize = ImageSize / Math.Max(BoardWidth, BoardHeight) - 1;
            int offsetX = (ImageSize - (CellSize * BoardWidth)) / 2;
            int offsetY = (ImageSize - (CellSize * BoardHeight)) / 2;

            for(int x = 0; x < BoardWidth; ++x)
                for(int y = 0; y < BoardHeight; ++y)
                {
                    Result.DrawRectangle(offsetX + (CellSize * x), offsetY + (CellSize * y), offsetX + (CellSize * x) + CellSize, offsetY + (CellSize * y) + CellSize, Colors.DarkGray);
                }

            Result.DrawRectangle(offsetX + (CellSize * Agents[0].X), offsetY + (CellSize * Agents[0].Y), offsetX + (CellSize * Agents[0].X) + CellSize, offsetY + (CellSize * Agents[0].Y) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetX + (CellSize * Agents[1].X), offsetY + (CellSize * Agents[1].Y), offsetX + (CellSize * Agents[1].X) + CellSize, offsetY + (CellSize * Agents[1].Y) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetX + (CellSize * Agents[2].X), offsetY + (CellSize * Agents[2].Y), offsetX + (CellSize * Agents[2].X) + CellSize, offsetY + (CellSize * Agents[2].Y) + CellSize, Colors.Red);
            Result.DrawRectangle(offsetX + (CellSize * Agents[3].X), offsetY + (CellSize * Agents[3].Y), offsetX + (CellSize * Agents[3].X) + CellSize, offsetY + (CellSize * Agents[3].Y) + CellSize, Colors.Red);

            return Result;
        }

    }

    public enum AgentPositioningState : byte
    {
        Error = 0, Horizontal, Vertical
    }

    [ValueConversion(typeof(Enum), typeof(bool))]
    public class AgentPositioningStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Binding.DoNothing;
            if ((bool)value)
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            return Binding.DoNothing;
        }
    }
}
