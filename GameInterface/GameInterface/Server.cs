using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTProcon29Protocol.Methods;
using MCTProcon29Protocol;

namespace GameInterface
{
    internal class ClientRennenend : IIPCServerReader
    {
        Server server;
        GameManager gameManager;
        private int managerNum;
        public ClientRennenend(Server server_, GameManager gameManager_, int managerNum_)
        {
            this.gameManager = gameManager_;
            this.managerNum = managerNum_;
            this.server = server_;
        }
        public void OnConnect(Connect connect)
        {
            MessageBox.Show("Connected.");
            server.SendGameInit(managerNum);
            throw new NotImplementedException();
        }

        public void OnDecided(Decided decided)
        {
            Agent.Direction dir = Agent.CastPointToDir(new Point((int)decided.MeAgent1.X, (int)decided.MeAgent1.Y));
            gameManager.OrderToAgent(new Order(managerNum * 2, dir, Agent.State.MOVE));
            dir = Agent.CastPointToDir(new Point((int)decided.MeAgent1.X, (int)decided.MeAgent1.Y));
            gameManager.OrderToAgent(new Order(managerNum * 2 + 1, dir, Agent.State.MOVE));
            throw new NotImplementedException();
        }

        public void OnInterrupt(Interrupt interrupt)
        {
            throw new NotImplementedException();
        }
    }

    class Server
    {
        IPCManager[] managers = new IPCManager[2];
        GameData data;
        public bool[] isConnected = new bool[] { false, false };

        public Server(GameManager gameManager)
        {
            //for (int i = 0; i < Constants.PlayersNum; i++)
            //{
            //    managers[i] = new IPCManager(new ClientRennenend(this, gameManager, i));
            //}
            //managers[0].Start(15000);
            //managers[1].Start(15001);
            //data = gameManager.data;
            //App.Current.Exit += (obj, e) =>
            //{
            //    foreach (var man in managers)
            //        man.ShutdownServer();
            //};
        }

        public void SendGameInit(int playerNum)
        {
            sbyte[,] board = new sbyte[data.BoardWidth, data.BoardHeight];
            for (int i = 0; i < data.BoardWidth; i++)
            {
                for (int j = 0; j < data.BoardHeight; j++)
                {
                    board[i, j] = (sbyte)data.CellData[i, j].Score;
                }
            }
            managers[playerNum].Write(DataKind.GameInit, new GameInit((byte)data.BoardHeight, (byte)data.BoardWidth, board,
                new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                GameData.FINISH_TURN));
            isConnected[playerNum] = true;
        }

        public void SendTurnStart(int playerNum)
        {
            if (!isConnected[playerNum]) return;

            ColoredBoardSmallBigger colorBoardMe = new ColoredBoardSmallBigger();
            ColoredBoardSmallBigger colorBoardEnemy = new ColoredBoardSmallBigger();

            for (int i = 0; i < data.BoardWidth; i++)
            {
                for (int j = 0; j < data.BoardHeight; j++)
                {
                    if (data.CellData[i, j].AreaState_ == TeamColor.Area1P)
                        colorBoardMe[(uint)i, (uint)j] = true;
                    else if (data.CellData[i, j].AreaState_ == TeamColor.Area2P)
                        colorBoardEnemy[(uint)i, (uint)j] = true;
                }
            }
            if (playerNum == 1) Swap(ref colorBoardMe, ref colorBoardEnemy);
            managers[playerNum].Write(DataKind.TurnStart, new TurnStart((byte)data.NowTurn, GameData.TIME_LIMIT_SECOND,
                new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                colorBoardMe,
                colorBoardEnemy));
        }

        public void SendTurnEnd(int playerNum)
        {
            if (!isConnected[playerNum]) return;
            managers[playerNum].Write(DataKind.TurnEnd, new TurnEnd((byte)data.NowTurn));
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
