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
    /// RotationDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RotationDialog : Window
    {
        public new RotationStateSelectViewModel DataContext;

        public RotationDialog(RotationStateSelectViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            base.DataContext = this.DataContext;
        }
        public static bool ShowDialog(out RotationState Result, GameSettings.SettingStructure settingStructure, AgentPositioningState state)
        {
            Result = RotationState.None;
            var vm = new RotationStateSelectViewModel();
            vm.Init(settingStructure.QCCell.GetLength(0), settingStructure.QCCell.GetLength(1), settingStructure.QCAgent, state);
            var dig = new RotationDialog(vm);
            if (dig.ShowDialog() == true)
            {
                Result = dig.DataContext.RotationState;
                if (Result == RotationState.None)
                    return false;
                return true;
            }
            return false;
        }

        public void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
    }

    public class RotationStateSelectViewModel : ViewModels.ViewModelBase
    {
        private const int ImageSize = 300;

        private WriteableBitmap rightResultBitmap = null;
        public WriteableBitmap RightResultBitmap {
            get => rightResultBitmap;
            set => RaisePropertyChanged(ref rightResultBitmap, value);
        }

        private WriteableBitmap leftResultBitmap = null;
        public WriteableBitmap LeftResultBitmap {
            get => leftResultBitmap;
            set => RaisePropertyChanged(ref leftResultBitmap, value);
        }

        private WriteableBitmap noneResultBitmap = null;
        public WriteableBitmap NoneResultBitmap {
            get => noneResultBitmap;
            set => RaisePropertyChanged(ref noneResultBitmap, value);
        }

        private RotationState rotationState = RotationState.None;
        public RotationState RotationState {
            get => rotationState;
            set => RaisePropertyChanged(ref rotationState, value);
        }

        public void Init(int BoardWidth, int BoardHeight, Agent[] Agents, AgentPositioningState state)
        {
            var enemy1 = new Point(Agents[0].Point.X, BoardHeight - Agents[0].Point.Y - 1);
            var enemy2 = new Point(Agents[1].Point.X, BoardHeight - Agents[1].Point.Y - 1);
            if (state == AgentPositioningState.Vertical)
            {
                enemy1 = new Point(BoardWidth - Agents[0].Point.X - 1, Agents[0].Point.Y);
                enemy2 = new Point(BoardWidth - Agents[1].Point.X - 1, Agents[1].Point.Y);
            }
            NoneResultBitmap = Draw(BoardWidth, BoardHeight, new[] { Agents[0].Point, Agents[1].Point, enemy1, enemy2 });
            LeftResultBitmap = DrawLeft(BoardWidth, BoardHeight, new[] { Agents[0].Point, Agents[1].Point, enemy1, enemy2 });
            RightResultBitmap = DrawRight(BoardWidth, BoardHeight, new[] { Agents[0].Point, Agents[1].Point, enemy1, enemy2 });
        }

        private WriteableBitmap Draw(int BoardWidth, int BoardHeight, Point[] Agents)
        {
            WriteableBitmap Result = new WriteableBitmap(ImageSize, ImageSize, 96, 96, PixelFormats.Bgra32, null);

            int CellSize = ImageSize / Math.Max(BoardWidth, BoardHeight) - 1;
            int offsetX = (ImageSize - (CellSize * BoardWidth)) / 2;
            int offsetY = (ImageSize - (CellSize * BoardHeight)) / 2;

            for (int x = 0; x < BoardWidth; ++x)
                for (int y = 0; y < BoardHeight; ++y)
                {
                    Result.DrawRectangle(offsetX + (CellSize * x), offsetY + (CellSize * y), offsetX + (CellSize * x) + CellSize, offsetY + (CellSize * y) + CellSize, Colors.DarkGray);
                }

            Result.DrawRectangle(offsetX + (CellSize * Agents[0].X), offsetY + (CellSize * Agents[0].Y), offsetX + (CellSize * Agents[0].X) + CellSize, offsetY + (CellSize * Agents[0].Y) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetX + (CellSize * Agents[1].X), offsetY + (CellSize * Agents[1].Y), offsetX + (CellSize * Agents[1].X) + CellSize, offsetY + (CellSize * Agents[1].Y) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetX + (CellSize * Agents[2].X), offsetY + (CellSize * Agents[2].Y), offsetX + (CellSize * Agents[2].X) + CellSize, offsetY + (CellSize * Agents[2].Y) + CellSize, Colors.Red);
            Result.DrawRectangle(offsetX + (CellSize * Agents[3].X), offsetY + (CellSize * Agents[3].Y), offsetX + (CellSize * Agents[3].X) + CellSize, offsetY + (CellSize * Agents[3].Y) + CellSize, Colors.Red);

            return Result;
        }

        private WriteableBitmap DrawLeft(int BoardWidth, int BoardHeight, Point[] Agents)
        {
            WriteableBitmap Result = new WriteableBitmap(ImageSize, ImageSize, 96, 96, PixelFormats.Bgra32, null);

            int CellSize = ImageSize / Math.Max(BoardWidth, BoardHeight) - 1;
            int offsetX = (ImageSize - (CellSize * BoardHeight)) / 2;
            int offsetY = (ImageSize - (CellSize * BoardWidth)) / 2;

            for (int x = BoardHeight - 1; x >= 0; --x)
                for (int y = 0; y < BoardWidth; ++y)
                {
                    Result.DrawRectangle(offsetX + (CellSize * x), offsetY + (CellSize * y), offsetX + (CellSize * x) + CellSize, offsetY + (CellSize * y) + CellSize, Colors.DarkGray);
                }

            Result.DrawRectangle(offsetY + (CellSize * (BoardHeight - 1 - Agents[0].Y)), offsetX + (CellSize * Agents[0].X), offsetY + (CellSize * (BoardHeight - 1 - Agents[0].Y)) + CellSize, offsetX + (CellSize * Agents[0].X) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetY + (CellSize * (BoardHeight - 1 - Agents[1].Y)), offsetX + (CellSize * Agents[1].X), offsetY + (CellSize * (BoardHeight - 1 - Agents[1].Y)) + CellSize, offsetX + (CellSize * Agents[0].X) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetY + (CellSize * (BoardHeight - 1 - Agents[2].Y)), offsetX + (CellSize * Agents[2].X), offsetY + (CellSize * (BoardHeight - 1 - Agents[2].Y)) + CellSize, offsetX + (CellSize * Agents[0].X) + CellSize, Colors.Red);
            Result.DrawRectangle(offsetY + (CellSize * (BoardHeight - 1 - Agents[3].Y)), offsetX + (CellSize * Agents[3].X), offsetY + (CellSize * (BoardHeight - 1 - Agents[3].Y)) + CellSize, offsetX + (CellSize * Agents[0].X) + CellSize, Colors.Red);
            return Result;
        }

        private WriteableBitmap DrawRight(int BoardWidth, int BoardHeight, Point[] Agents)
        {
            WriteableBitmap Result = new WriteableBitmap(ImageSize, ImageSize, 96, 96, PixelFormats.Bgra32, null);

            int CellSize = ImageSize / Math.Max(BoardWidth, BoardHeight) - 1;
            int offsetX = (ImageSize - (CellSize * BoardHeight)) / 2;
            int offsetY = (ImageSize - (CellSize * BoardWidth)) / 2;

            for (int x = 0; x < BoardHeight; ++x)
                for (int y = BoardWidth - 1; y >= 0; --y)
                {
                    Result.DrawRectangle(offsetX + (CellSize * x), offsetY + (CellSize * y), offsetX + (CellSize * x) + CellSize, offsetY + (CellSize * y) + CellSize, Colors.DarkGray);
                }

            Result.DrawRectangle(offsetY + (CellSize * Agents[0].Y), offsetX + (CellSize * (BoardWidth - 1 - Agents[0].X)), offsetY + (CellSize * Agents[0].Y) + CellSize, offsetX + (CellSize * (BoardWidth - 1 - Agents[0].X)) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetY + (CellSize * Agents[1].Y), offsetX + (CellSize * (BoardWidth - 1 - Agents[1].X)), offsetY + (CellSize * Agents[1].Y) + CellSize, offsetX + (CellSize * (BoardWidth - 1 - Agents[1].X)) + CellSize, Colors.Blue);
            Result.DrawRectangle(offsetY + (CellSize * Agents[2].Y), offsetX + (CellSize * (BoardWidth - 1 - Agents[2].X)), offsetY + (CellSize * Agents[2].Y) + CellSize, offsetX + (CellSize * (BoardWidth - 1 - Agents[2].X)) + CellSize, Colors.Red);
            Result.DrawRectangle(offsetY + (CellSize * Agents[3].Y), offsetX + (CellSize * (BoardWidth - 1 - Agents[3].X)), offsetY + (CellSize * Agents[3].Y) + CellSize, offsetX + (CellSize * (BoardWidth - 1 - Agents[3].X)) + CellSize, Colors.Red);
            return Result;
        }
    }

    public enum RotationState : byte
    {
        None = 0, Right, Left
    }

    [ValueConversion(typeof(Enum), typeof(bool))]
    public class RotationStateConverter : IValueConverter
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
