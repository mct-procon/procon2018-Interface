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
        public GameData Data;
        internal Server Server;
        private DispatcherTimer dispatcherTimer;
        public MainWindowViewModel viewModel;

        public GameManager(MainWindowViewModel _viewModel)
        {
            this.viewModel = _viewModel;
            this.Data = new GameData(_viewModel);
            this.Server = new Server(this);
        }

        public void StartGame()
        {
            Server.SendGameInit();
            InitDispatcherTimer();
            ResetOrder();
            ScoreCalculator.Init((uint)Data.BoardHeight, (uint)Data.BoardWidth);
            StartTurn();
            GetScore();
            Data.IsGameStarted = true;
        }

        public void EndGame()
        {
            TimerStop();
        }

        public void PauseGame()
        {
            Data.IsPause = true;
        }

        public void RerunGame()
        {
            Data.IsPause = false;
        }

        public void InitGameData(GameSettings.SettingStructure settings)
        {
            Data.InitGameData(settings);
            Data.IsPause = false;
            Server.StartListening(settings);
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
            viewModel.TurnStr = $"TURN:{Data.NowTurn}/{Data.FinishTurn}";
            viewModel.TimerStr = $"TIME:{Data.SecondCount}/{Data.TimeLimitSeconds}";
        }

        //一秒ごとに呼ばれる
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            if (!Data.IsNextTurnStart) return;
            Data.SecondCount++;
            if (Data.SecondCount == Data.TimeLimitSeconds || Server.IsDecidedReceived.All(b => b))
            {
                Data.IsNextTurnStart = false;
                EndTurn();
            }
        }

        public void StartTurn()
        {
            Data.IsNextTurnStart = true;
            MoveAgents();
            GetScore();
            Data.SecondCount = 0;
            Server.SendTurnStart();
        }

        public void EndTurn()
        {
            if (!Data.IsGameStarted) return;
            Server.SendTurnEnd();
            if (Data.NowTurn < Data.FinishTurn)
            {
                Data.NowTurn++;
                if (Data.IsAutoSkipTurn)
                    StartTurn();
            }
            else
            {
                Server.SendGameEnd(true);
            }
        }

        public void ChangeCellToNextColor(Point point)
        {
            //エージェントがいる場合、エージェントの移動処理へ
            int onAgnetNum = IsOnAgent(point);
            if (onAgnetNum != -1)
            {
                Data.SelectPosAgent = onAgnetNum;
                return;
            }
            else
            {
                for (int i = 0; i < Constants.AgentsNum; i++)
                {
                    if (Data.SelectPosAgent == i)
                    {
                        Data.SelectPosAgent = -1;
                        WarpAgent(Data.Agents[i], point);
                        return;
                    }
                }
            }

            var color = Data.CellData[point.X, point.Y].AreaState_;
            var nextColor = (TeamColor)(((int)color + 1) % 3);
            Data.CellData[point.X, point.Y].AreaState_ = nextColor;
        }

        private int IsOnAgent(Point point)
        {
            for (int i = 0; i < Constants.AgentsNum; i++)
            {
                var agentPoint = Data.Agents[i].Point;
                if (agentPoint.X == point.X && agentPoint.Y == point.Y)
                {
                    return i;
                }
            }
            return -1;
        }

        private void WarpAgent(Agent agent, Point point)
        {
            Data.CellData[agent.Point.X, agent.Point.Y].AgentState = TeamColor.Free;
            agent.Point = point;
            var nextPointColor =
                agent.playerNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            Data.CellData[point.X, point.Y].AreaState_ = nextPointColor;
            Data.CellData[point.X, point.Y].AgentState = nextPointColor;
            viewModel.Agents = Data.Agents;
            return;
        }

        private void MoveAgents()
        {
            List<int> ActionableAgentsId = GetActionableAgentsId();

            // Erase Agent Location's data from cells.
            foreach (var a in Data.Agents)
                Data.CellData[a.Point.X, a.Point.Y].AgentState = TeamColor.Free;

            for (int i = 0; i < ActionableAgentsId.Count; i++)
            {
                int id = ActionableAgentsId[i];
                var agent = Data.Agents[id];
                var nextP = agent.GetNextPoint();

                TeamColor nextAreaState = Data.CellData[nextP.X, nextP.Y].AreaState_;
                ActionAgentToNextP(id, agent, nextP, nextAreaState);
                viewModel.IsRemoveMode[id] = false;
            }
            viewModel.Agents = Data.Agents;

            // Reset Agent Location's data to cells.
            for (int id = 0; id < Data.Agents.Length; ++id)
            {
                var a = Data.Agents[id].Point;
                Data.CellData[a.X, a.Y].AgentState = id / Constants.PlayersNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;
            }
        }

        //naotti: 行動可能なエージェントのId(1p{0,1}, 2p{2,3})を返す。
        private List<int> GetActionableAgentsId()
        {
            int i, j;
            bool[] canMove = new bool[Constants.AgentsNum];     //canMove[i] = エージェントiは移動するか？
            bool[] canAction = new bool[Constants.AgentsNum];   //canAction[i] = エージェントiは移動またはタイル除去をするか？

            //まずは、各エージェントの移動先を知りたいので、canMoveを求める。
            //最初, canMove[i] = trueとしておき、移動不可なエージェントを振るい落とす方式を取る。このループでは以下の2点をチェックする。
            //・相手陣を指しているエージェントはタイル除去なので、移動しない
            //・範囲外を指しているエージェントは移動できない。
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                canMove[i] = true;
                var agent = Data.Agents[i];
                var nextP = agent.GetNextPoint();
                if (CheckIsPointInBoard(nextP) == false) { canMove[i] = false; continue; }
                TeamColor nextAreaState = Data.CellData[nextP.X, nextP.Y].AreaState_;
                if (agent.playerNum == 0 && nextAreaState == TeamColor.Area2P) { canMove[i] = false; }
                if (agent.playerNum == 1 && nextAreaState == TeamColor.Area1P) { canMove[i] = false; }
            }

            //次に、「指示先(agent.GetNextPoint()の位置)が被っているエージェントは移動不可」とする。
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                var agent1 = Data.Agents[i];
                var nextP1 = agent1.GetNextPoint();
                for (j = i + 1; j < Constants.AgentsNum; j++)
                {
                    var agent2 = Data.Agents[j];
                    var nextP2 = agent2.GetNextPoint();
                    if (nextP1.CompareTo(nextP2) == 0)
                    {
                        canMove[i] = false;
                        canMove[j] = false;
                    }
                }
            }

            //次に、canMove[]の更新が起きなくなるまで、以下を繰り返す
            //・移動先に移動不可な(orタイル除去をする)エージェントがいる場合、移動不可とする
            while (true)
            {
                bool updateFlag = false;

                for (i = 0; i < Constants.AgentsNum; i++)
                {
                    if (canMove[i] == false) { continue; }
                    var agent1 = Data.Agents[i];
                    var nextP1 = agent1.GetNextPoint();
                    for (j = 0; j < Constants.AgentsNum; j++)
                    {
                        if (i == j) { continue; }
                        if (canMove[j] == true) { continue; }
                        var agent2 = Data.Agents[j];
                        var nextP2 = agent2.Point;
                        if (nextP1.CompareTo(nextP2) == 0)
                        {
                            canMove[i] = false;
                            updateFlag = true;
                            break;
                        }
                    }
                }
                if (updateFlag == false) { break; }
            }

            //この時点でcanMove[i] == trueならば、エージェントiは移動することになる。
            //次は、行動(移動またはタイル除去)が可能なエージェントを求める。
            //最初, canAction[i] = trueとしておき、行動不可なエージェントを振るい落とす方式を取る。このループでは以下の1点をチェックする。
            //・範囲外を指しているエージェントは移動できない。
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                canAction[i] = true;
                var agent = Data.Agents[i];
                var nextP = agent.GetNextPoint();
                if (CheckIsPointInBoard(nextP) == false) { canAction[i] = false; }
            }

            //次に、「指示先(agent.GetNextPoint()の位置)が被っているエージェントは行動不可」とする。
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                var agent1 = Data.Agents[i];
                var nextP1 = agent1.GetNextPoint();
                for (j = i + 1; j < Constants.AgentsNum; j++)
                {
                    var agent2 = Data.Agents[j];
                    var nextP2 = agent2.GetNextPoint();
                    if (nextP1.CompareTo(nextP2) == 0)
                    {
                        canAction[i] = false;
                        canAction[j] = false;
                    }
                }
            }

            //次に、「指示先に移動不可な(orタイル除去をする)エージェントがいる場合、行動不可」とする。
            //このチェックは, 先ほどのように何回もwhileループで回す必要がない。
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                if (canAction[i] == false) { continue; }
                var agent1 = Data.Agents[i];
                var nextP1 = agent1.GetNextPoint();

                for (j = 0; j < Constants.AgentsNum; j++)
                {
                    if (i == j) { continue; }
                    if (canMove[j] == true) { continue; }
                    var agent2 = Data.Agents[j];
                    var nextP2 = agent2.Point;

                    if (nextP1.CompareTo(nextP2) == 0)
                    {
                        canAction[i] = false;
                        break;
                    }
                }
            }

            //この時点でcanAction[i] == trueならば、エージェントiは行動可能である
            //よって、行動可能なエージェントの番号を返すことができる
            List<int> ret = new List<int>();
            for (i = 0; i < Constants.AgentsNum; i++)
            {
                if (canAction[i]) { ret.Add(i); }
                //else { MessageBox.Show(i.ToString()); }
            }
            return ret;
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
            for (int x = 0; x < Data.CellData.GetLength(0); ++x)
                for (int y = 0; y < Data.CellData.GetLength(1); ++y)
                    Data.CellData[x, y].SurroundedState = TeamColor.Free;
            for (int i = 0; i < Constants.PlayersNum; i++)
                Data.PlayerScores[i] = ScoreCalculator.CalcScore(i, Data.CellData);
            viewModel.PlayerScores = Data.PlayerScores;
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
                                Data.Agents[i].Point = nextP;
                                Data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Area1P;
                            }
                            else
                            {
                                Data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                            }
                            break;
                        case 1:
                            if (nextAreaState != TeamColor.Area1P)
                            {
                                Data.Agents[i].Point = nextP;
                                Data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Area2P;
                            }
                            else
                            {
                                Data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                            }
                            break;
                    }
                    break;
                case Agent.State.REMOVE_TILE:
                    Data.CellData[nextP.X, nextP.Y].AreaState_ = TeamColor.Free;
                    break;
                default:
                    break;
            }
            agent.AgentDirection = Agent.Direction.NONE;
            agent.AgentState = Agent.State.MOVE;
        }

        private void Draw()
        {
            viewModel.TimerStr = $"TIME:{Data.SecondCount}/{Data.TimeLimitSeconds}";
            viewModel.TurnStr = $"TURN:{Data.NowTurn}/{Data.FinishTurn}";
        }

        public void OrderToAgent(Order order)
        {
            Data.Agents[order.agentNum].AgentState = order.state;
            Data.Agents[order.agentNum].AgentDirection = order.direction;
            viewModel.Agents = Data.Agents;
        }

        private void ResetOrder()
        {
            foreach (var agent in Data.Agents )
            {
                agent.AgentDirection = Agent.Direction.NONE;
                agent.AgentState = Agent.State.MOVE;
            }
            viewModel.Agents = Data.Agents;
        }

        private bool CheckIsPointInBoard(Point p)
        {
            return (p.X >= 0 && p.X < Data.BoardWidth &&
                p.Y >= 0 && p.Y < Data.BoardHeight);
        }
    }
}
