﻿using GameInterface.Cells;
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
        public byte FinishTurn { get; set; } = 60;
        public int TimeLimitSeconds { get; set; } = 5;

        private MainWindowViewModel viewModel;
        private System.Random rand = new System.Random();
        //---------------------------------------
        //ViewModelと連動させるデータ(画面上に現れるデータ)
        private Cell[,] cellDataValue = null;
        public Cell[,] CellData
        {
            get => cellDataValue;
            private set
            {
                cellDataValue = value;
                viewModel.CellData = value;
            }
        }
        public Agent[] Agents
        {
            get => viewModel.Agents;
            set => viewModel.Agents = value;
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
        //----------------------------------------
        //それ以外
        public int SecondCount { get; set; }
        public bool isStarted = false;
        public int NowTurn { get; set; }
        public int BoardHeight { get; private set; }
        public int BoardWidth { get; private set; }

        public GameData(MainWindowViewModel _viewModel)
        {
            viewModel = _viewModel;
        }

        public void InitGameData( GameSettings.SettingStructure settings)
        {
            SecondCount = 0;
            NowTurn = 0;
            FinishTurn = settings.Turns;
            TimeLimitSeconds = settings.LimitTime;
            InitCellData(settings);
            InitAgents();
        }

        void InitCellData(GameSettings.SettingStructure settings)
        {
            BoardHeight = settings.BoardHeight;
            BoardWidth = settings.BoardWidth;

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

            CellData = new Cell[BoardWidth, BoardHeight];
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    if (i < randWidth && j < randHeight)
                    {
                        //10%の確率で値を0以下にする
                        if (rand.Next(1, 100) > 10)
                            CellData[i, j] = new Cell(rand.Next(1, 14));
                        else
                            CellData[i, j] = new Cell(rand.Next(-14, 0));
                    }
                    else if (j >= randHeight)
                        CellData[i, j] = new Cell(CellData[i, BoardHeight - 1 - j].Score);
                    else
                        CellData[i, j] = new Cell(CellData[BoardWidth - 1 - i, j].Score);
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
                Agents[i].playerNum = (i / Constants.PlayersNum);
                Agents[i].Point = new Point(agentsX[i], agentsY[i]);
                CellData[agentsX[i], agentsY[i]].AreaState_ =
                    i / Constants.PlayersNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;

                CellData[agentsX[i], agentsY[i]].AgentState =
                    i / Constants.PlayersNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            }
        }
    }
}
