using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTProcon29Protocol.Methods;

namespace GameInterface
{
    internal class ClientRennenend : MCTProcon29Protocol.IIPCServerReader
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
        MCTProcon29Protocol.IPCManager[] managers = new MCTProcon29Protocol.IPCManager[2];
        GameData data;
        public bool[] isConnected = new bool[] { false, false };

        public Server(GameManager gameManager)
        {
            for (int i = 0; i < Constants.PlayersNum; i++)
            {
                managers[i] = new MCTProcon29Protocol.IPCManager(new ClientRennenend(this, gameManager, i));
            }
            managers[0].Start(15000);
            managers[1].Start(15001);
            data = gameManager.data;
        }

        public void SendGameInit(int playerNum)
        {
            sbyte[,] board = new sbyte[data.BoardHeight, data.BoardWidth];
            for (int i = 0; i < data.BoardHeight; i++)
            {
                for (int j = 0; j < data.BoardWidth; j++)
                {
                    board[i, j] = (sbyte)data.CellData[i][j].Score;
                }
            }
            managers[playerNum].Write(DataKind.GameInit, new GameInit()
            {
                TeamId = (byte)playerNum,
                BoardHeight = (byte)data.BoardHeight,
                BoardWidth = (byte)data.BoardWidth,
                Board = board,
                MeAgent1 = new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                MeAgent2 = new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                EnemyAgent1 = new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                EnemyAgent2 = new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                Turns = GameData.FINISH_TURN,
            });
            isConnected[playerNum] = true;
        }

        public void SendTurnStart(int playerNum)
        {
            if (!isConnected[playerNum]) return;
            ushort[] colorBoardMe = new ushort[data.BoardHeight];
            ushort[] colorBoardEnemy = new ushort[data.BoardHeight];
            for (int i = 0; i < data.BoardHeight; i++)
            {
                for (int j = 0; j < data.BoardWidth; j++)
                {
                    if (data.CellData[i][j].AreaState_ == Cell.AreaState.AREA_1P)
                        colorBoardMe[i] += (ushort)(1 << j);
                }
            }
            for (int i = 0; i < data.BoardHeight; i++)
            {
                for (int j = 0; j < data.BoardWidth; j++)
                {
                    if (data.CellData[i][j].AreaState_ == Cell.AreaState.AREA_2P)
                        colorBoardEnemy[i] += (ushort)(1 << j);
                }
            }
            if (playerNum == 1) Swap(ref colorBoardMe, ref colorBoardEnemy);
            managers[playerNum].Write(DataKind.TurnStart, new TurnStart
            {
                Turn = (byte)data.NowTurn,
                WaitMiliSeconds = GameData.TIME_LIMIT_SECOND * 1000,
                MeAgent1 = new MCTProcon29Protocol.Point((uint)data.Agents[0 + playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                MeAgent2 = new MCTProcon29Protocol.Point((uint)data.Agents[1 + playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                EnemyAgent1 = new MCTProcon29Protocol.Point((uint)data.Agents[2 - playerNum * 2].Point.X, (uint)data.Agents[0 + playerNum * 2].Point.Y),
                EnemyAgent2 = new MCTProcon29Protocol.Point((uint)data.Agents[3 - playerNum * 2].Point.X, (uint)data.Agents[1 + playerNum * 2].Point.Y),
                MeColoredBoard = colorBoardMe,
                EnemyColoredBoard = colorBoardEnemy,
            });
        }

        public void SendTurnEnd(int playerNum)
        {
            if (!isConnected[playerNum]) return;
            managers[playerNum].Write(DataKind.TurnEnd, new TurnEnd()
            {
                Turn = (byte)data.NowTurn,
            });
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
