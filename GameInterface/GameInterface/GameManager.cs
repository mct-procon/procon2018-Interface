using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GameInterface
{
    public class GameManager
    {
        public GameData data;
        private Server server;
        private DispatcherTimer dispatcherTimer;
        private MainWindowViewModel viewModel;


        public GameManager(MainWindowViewModel _viewModel)
        {
            this.viewModel = _viewModel;
            InitGameData(_viewModel);
            InitDispatcherTimer();
        }
        private void InitGameData(MainWindowViewModel _viewModel)
        {
            this.data = new GameData(_viewModel);
            this.server = new Server(this);
        }
        private void InitDispatcherTimer()
        {
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        //一秒ごとに呼ばれる
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            if (!data.isStarted)
            {
                server.SendTurnStart(0);
                server.SendTurnStart(1);
                data.isStarted = true;
            }
            data.SecondCount++;
            if (data.SecondCount % (GameData.TIME_LIMIT_SECOND) == 0)
            {
                data.NowTurn++;
                server.SendTurnEnd(0);
                server.SendTurnEnd(1);
                data.SecondCount = 0;
                MoveAgents();
                GetScore();
                server.SendTurnStart(0);
                server.SendTurnStart(1);
            }
        }

        private void MoveAgents()
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                Agent agent = data.Agents[i];

                data.CellData[agent.Point.X, agent.Point.Y].AgentState = TeamColor.Free;

                Point nextP = agent.GetNextPoint();
                if (CheckIsPointInBoard(nextP) == false) continue;
                TeamColor nextAreaState = data.CellData[nextP.X, nextP.Y].AreaState_;
                ActionAgentToNextP(i, agent, nextP, nextAreaState);
                viewModel.IsRemoveMode[i] = false;

                data.CellData[agent.Point.X, agent.Point.Y].AgentState =
                    i / Constants.PlayersNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            }
            viewModel.Agents = data.Agents;
        }

        private void GetScore()
        {
            for (int i = 0; i < Constants.PlayersNum; i++)
                data.PlayerScores[i] = ScoreCalculator.CalcScore(i, data.BoardHeight, data.BoardWidth, data.CellData);
            viewModel.PlayerScores = data.PlayerScores;
        }

        private void ActionAgentToNextP(int i, Agent agent, Point nextP, TeamColor nextAreaState)
        {
            switch (agent.AgentState)
            {
                case Agent.State.MOVE:
                    switch (agent.playerNum)
                    {
                        case 0:
                            if (nextAreaState != TeamColor.Area2P)
                            {
                                data.Agents[i].Point = nextP;
                                data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Area1P;
                            }
                            else
                            {
                                data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                            }
                            break;
                        case 1:
                            if (nextAreaState != TeamColor.Area1P)
                            {
                                data.Agents[i].Point = nextP;
                                data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Area2P;
                            }
                            else
                            {
                                data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                            }
                            break;
                    }
                    break;
                case Agent.State.REMOVE_TILE:
                    data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                    break;
                default:
                    break;
            }
            agent.AgentDirection = Agent.Direction.NONE;
            agent.AgentState = Agent.State.MOVE;
        }

        private void Draw()
        {
            data.TimerStr = "TIME:" + data.SecondCount.ToString() + "/" + GameData.TIME_LIMIT_SECOND.ToString();
            data.TurnStr = "TURN:" + data.NowTurn.ToString() + "/" + GameData.FINISH_TURN.ToString();
        }

        public void OrderToAgent(Order order)
        {
            data.Agents[order.agentNum].AgentState = order.state;
            data.Agents[order.agentNum].AgentDirection = order.direction;
            viewModel.Agents = data.Agents;
        }

        private bool CheckIsPointInBoard(Point p)
        {
            return (p.X >= 0 && p.X < data.BoardWidth &&
                p.Y >= 0 && p.Y < data.BoardHeight);
        }
    }
}
