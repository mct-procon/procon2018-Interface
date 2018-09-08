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
        internal Server server;
        private DispatcherTimer dispatcherTimer;
        public MainWindowViewModel viewModel;

        public GameManager(MainWindowViewModel _viewModel)
        {
            this.viewModel = _viewModel;
            this.data = new GameData(_viewModel);
            this.server = new Server(this);
        }

        public void StartGame()
        {
            server.SendGameInit();
            InitDispatcherTimer();
            StartTurn();
            GetScore();
            data.IsGameStarted = true;
        }

        public void EndGame()
        {
            TimerStop();
        }

        public void InitGameData(GameSettings.SettingStructure settings)
        {
            data.InitGameData(settings);
            server.StartListening(settings);
        }

        public void TimerStop()
        {
            dispatcherTimer?.Stop();
        }

        public void TimerResume()
        {
            dispatcherTimer?.Start();
        }

        private void InitDispatcherTimer()
        {
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Start();
            viewModel.TurnStr = $"TURN:{data.NowTurn}/{data.FinishTurn}";
            viewModel.TimerStr = $"TIME:{data.SecondCount}/{data.TimeLimitSeconds}";
        }

        //一秒ごとに呼ばれる
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            if (!data.IsNextTurnStart) return;
            data.SecondCount++;
            if (data.SecondCount == data.TimeLimitSeconds)
            {
                EndTurn();
                data.IsNextTurnStart = false;
            }
        }

        public void StartTurn()
        {
            server.SendTurnStart();
            data.IsNextTurnStart = true;
            MoveAgents();
            data.SecondCount = 0;
        }

        public void EndTurn()
        {
            if (!data.IsGameStarted) return;
            server.SendTurnEnd();
            data.NowTurn++;
            GetScore();
        }

        public void ChangeCellToNextColor(Point point)
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                if (data.IsSelectPosMode[i])
                {
                    var agent = data.Agents[i];
                    data.CellData[agent.Point.X, agent.Point.Y].AgentState = TeamColor.Free;
                    data.Agents[i].Point = point;
                    var nextPointColor =
                        data.Agents[i].playerNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
                    data.CellData[point.X, point.Y].AreaState_ = nextPointColor;
                    data.CellData[point.X, point.Y].AgentState = nextPointColor;
                    data.IsSelectPosMode[i] = false;
                    return;
                }
            }
            int onAgnetNum = IsOnAgent(point);
            if (onAgnetNum != -1)
            {
                data.IsSelectPosMode[onAgnetNum] = true;
                return;
            }
            var color = data.CellData[point.X, point.Y].AreaState_;
            var nextColor = (TeamColor)(((int)color + 1) % 3);
            data.CellData[point.X, point.Y].AreaState_ = nextColor;
        }

        private int IsOnAgent(Point point)
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                var agentPoint = data.Agents[i].Point;
                if (agentPoint.X == point.X && agentPoint.Y == point.Y)
                {
                    return i;
                }
            }
            return -1;
        }

        private void MoveAgents()
        {
            Point[] nextPoints = new Point[] { new Point(-1, -1), new Point(-1, -1), new Point(-1, -1), new Point(-1, -1) };
            bool[] canMove = new bool[4];
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                var agent = data.Agents[i];
                var nextP = agent.GetNextPoint();
                if (CheckContain(nextPoints, nextP))
                {
                    canMove[i] = false;
                    MessageBox.Show(i.ToString());
                    for (int j = 0; j < i; j++)
                    {
                        if (nextPoints[j].CompareTo(nextP) == 0)
                        {
                            MessageBox.Show(j.ToString());
                            canMove[j] = false;
                        }
                    }
                }
                else canMove[i] = true;
                nextPoints[i] = nextP;
            }
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                var agent = data.Agents[i];
                var nextP = nextPoints[i];
                if (CheckIsPointInBoard(nextP) == false || !canMove[i]) continue;

                data.CellData[agent.Point.X, agent.Point.Y].AgentState = TeamColor.Free;
                TeamColor nextAreaState = data.CellData[nextP.X, nextP.Y].AreaState_;
                ActionAgentToNextP(i, agent, nextP, nextAreaState);
                viewModel.IsRemoveMode[i] = false;

                data.CellData[agent.Point.X, agent.Point.Y].AgentState =
                    i / Constants.PlayersNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            }
            viewModel.Agents = data.Agents;
        }

        private bool CheckContain(Point[] points, Point checkPoint)
        {
            foreach (var point in points)
            {
                if (point.CompareTo(checkPoint) == 0) return true;
            }
            return false;
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
            viewModel.TimerStr = $"TIME:{data.SecondCount}/{data.TimeLimitSeconds}";
            viewModel.TurnStr = $"TURN:{data.NowTurn}/{data.FinishTurn}";
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
