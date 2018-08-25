using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameInterface
{
    public class GameData
    {
        public const int FINISH_TURN = 60;
        public const int TIME_LIMIT_SECOND = 5;

        private MainWindowViewModel viewModel;
        private System.Random rand = new System.Random();
        //---------------------------------------
        //ViewModelと連動させるデータ(画面上に現れるデータ)
        private List<Cell>[] cellDataValue = new List<Cell>[12];
        public List<Cell>[] CellData
        {
            get => cellDataValue;
            private set
            {
                cellDataValue = value;
                viewModel.CellData = value;
            }
        }
        private String timerStr;
        public String TimerStr
        {
            get => timerStr;
            set
            {
                timerStr = value;
                viewModel.TimerStr = value;
            }
        }
        private Agent[] agents = new Agent[]{
            new Agent(),new Agent(),new Agent(),new Agent()
        };
        public Agent[] Agents
        {
            get => agents;
            set
            {
                agents = value;
                viewModel.Agents = value;
            }
        }
        private int[] playerScores = new int[2];
        public int[] PlayerScores
        {
            get => playerScores;
            set
            {
                playerScores = value;
                viewModel.PlayerScores = value;
            }
        }
        private String trunStr;
        public String TurnStr
        {
            get => trunStr;
            set
            {
                trunStr = value;
                viewModel.TurnStr = value;
            }
        }
        //----------------------------------------
        //それ以外
        public int SecondCount { get; set; }
        public bool isStarted = false;
        public int NowTurn { get; set; }
        public int BoardHeight { get; private set; }
        public int BoardWidth { get; private set; }

        public GameData(MainWindowViewModel _viewModel)
        {
            InitCellData();
            InitAgents();
            InitGameData(_viewModel);

        }
        void InitCellData()
        {

            while (true)
            {
                BoardHeight = rand.Next(7, 13);
                BoardWidth = rand.Next(7, 13);
                if (BoardHeight * BoardWidth >= 80) break;
            }

            //水平方向か垂直方向のどちらかを対称にするフラグをランダムに立てる
            bool isVertical;
            bool isHorizontal;
            do
            {
                isVertical = rand.Next(2) == 1 ? true : false;
                isHorizontal = rand.Next(2) == 1 ? true : false;
            } while (!isVertical && !isHorizontal);

            int randWidth, randHeight;
            randWidth = (isHorizontal) ? (BoardWidth + 1) / 2 : BoardWidth;
            randHeight = (isVertical) ? (BoardHeight + 1) / 2 : BoardHeight;

            for (int i = 0; i < BoardHeight; i++)
            {
                //リストの配列は宣言時はNullだから、インスタンスを入れて初期化
                CellData[i] = new List<Cell>();
                for (int j = 0; j < BoardWidth; j++)
                {
                    if (j < randWidth && i < randHeight)
                    {
                        //10%の確率で値を0以下にする
                        if (rand.Next(1, 100) > 10)
                            CellData[i].Add(new Cell(rand.Next(1, 14)));
                        else
                            CellData[i].Add(new Cell(rand.Next(-14, 0)));
                    }
                    else if (i >= randHeight)
                    {
                        CellData[i].Add(new Cell(CellData[BoardHeight - 1 - i][j].Score));
                    }
                    else
                    {
                        CellData[i].Add(new Cell(CellData[i][BoardWidth - 1 - j].Score));
                    }
                }
            }
        }

        void InitAgents()
        {
            /*
            配置は
            0 2
            3 1
            */
            int[] agentsX = new int[4];
            int[] agentsY = new int[4];
            agentsX[0] = agentsX[3] = rand.Next(1, BoardWidth / 2 - 1);
            agentsY[0] = agentsY[2] = rand.Next(1, BoardHeight / 2 - 1);
            agentsX[2] = agentsX[1] = BoardWidth - 1 - agentsX[0];
            agentsY[3] = agentsY[1] = BoardHeight - 1 - agentsY[0];
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                agents[i].playerNum = (i / Constants.PlayersNum);
                agents[i].Point = new Point(agentsX[i], agentsY[i]);
                CellData[agentsY[i]][agentsX
                    [i]].AreaState_ =
                    i / Constants.PlayersNum == 0 ? Cell.AreaState.AREA_1P : Cell.AreaState.AREA_2P;
            }
        }
        void InitGameData(MainWindowViewModel _viewModel)
        {
            viewModel = _viewModel;
            TimerStr = "TIME:0/10";
            TurnStr = "TURN:0/60";
            SecondCount = 0;
        }
    }
}
