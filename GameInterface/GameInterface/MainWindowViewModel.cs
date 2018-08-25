﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using GameInterface.Cells;

namespace GameInterface
{
    public class MainWindowViewModel : ViewModelBase
    {
        public GameManager gameManager;
        //---------------------------------------
        //画面に表示する変数
        private List<Cell>[] cellData = new List<Cell>[12];
        public List<Cell>[] CellData
        {
            get => cellData; 
            set
            {
                cellData = value;
                RaisePropertyChanged("CellData");
                RaisePropertyChanged("AreaState_");
            }
        }
        private String timerStr;
        public String TimerStr
        {
            get => timerStr; 
            set
            {
                timerStr = value;
                RaisePropertyChanged("TimerStr");
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
                UpdateOrderButton();
                RaisePropertyChanged("Agents");
                RaisePropertyChanged("Point");
            }
        }
        public OrderButtonUserControl[,] orderButtonUserControls = new OrderButtonUserControl[4, 9];
        private int[] playerScores = new int[2];
        public int[] PlayerScores
        {
            get => playerScores;
            set
            {
                playerScores = value;
                RaisePropertyChanged("PlayerScores");
            }
        }
        private bool[] isRemoveMode =new bool[4];
        public bool[] IsRemoveMode {
            get => isRemoveMode;
            set => isRemoveMode = value;
        }
        private String turnStr;
        public String TurnStr
        {
            get => turnStr;
            set
            {
                turnStr = value;
                RaisePropertyChanged("TurnStr");
            }
        }
        //---------------------------------------
        //ボタン等を押された時の関数
        public DelegateCommand<Order> OrderToAgentCommand { get; private set; }

        public MainWindowViewModel()
        {
            InitCommands();
        }
        private void InitCommands()
        {
            OrderToAgentCommand = new DelegateCommand<Order>(
                OrderToAgentFromVM
            );
        }

        private void OrderToAgentFromVM(Order order)
        {
            if (isRemoveMode[order.agentNum]) order.state = Agent.State.REMOVE_TILE;
            gameManager.OrderToAgent(order);
        }

        private void UpdateOrderButton()
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                for (int j = 0; j < Constants.OrderButtonsNum; j++)
                {
                    orderButtonUserControls[i, j].IsEnabled = true;
                }
                orderButtonUserControls[i, agents[i].GetDirectionIdFromDirection()].IsEnabled = false;
            }
        }
    }
}
