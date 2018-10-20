using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameInterface.Cells;
using MCTProcon29Protocol;

namespace GameInterface
{
    static class ScoreCalculator
    {
        private static uint height;
        private static uint width;
        private static readonly int[] DirectionX = new int[] { 1, 0, -1, 0 };
        private static readonly int[] DirectionY = new int[] { 0, 1, 0, -1 };

        public static void Init(uint height_, uint width_)
        {
            height = height_;
            width = width_;
        }

        public static int CalcScore(int playerNum, Cell[,] cells)
        {
            ColoredBoardSmallBigger checker = new ColoredBoardSmallBigger(width, height);
            int result = 0;
            var state = playerNum == 0 ? TeamColor.Area1P : TeamColor.Area2P;

            for (uint x = 0; x < width; ++x)
                for (uint y = 0; y < height; ++y)
                {
                    if (cells[x, y].AreaState_ == state)
                    {
                        result += cells[x, y].Score;
                        checker[x, y] = true;
                    }
                }
            BadSpaceFill(ref checker, width, height);

            for (uint x = 0; x < width; ++x)
                for (uint y = 0; y < height; ++y)
                    if (!checker[x, y])
                    {
                        result += Math.Abs(cells[x, y].Score);
                        cells[x, y].SurroundedState |= state;
                    }
            return result;
        }

        //uint[] myStack = new uint[1024];	//x, yの順で入れる. y, xの順で取り出す. width * height以上のサイズにする.
        public static unsafe void BadSpaceFill(ref ColoredBoardSmallBigger Checker, uint width, uint height)
        {
            unchecked
            {
                MCTProcon29Protocol.Point* myStack = stackalloc MCTProcon29Protocol.Point[12 * 12];

                MCTProcon29Protocol.Point point;
                uint x, y, searchTo = 0, myStackSize = 0;

                searchTo = height - 1;
                for (x = 0; x < width; x++)
                {
                    if (!Checker[x, 0])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(x, 0);
                        Checker[x, 0] = true;
                    }
                    if (!Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }
                }

                searchTo = width - 1;
                for (y = 0; y < height; y++)
                {
                    if (!Checker[0, y])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(0, y);
                        Checker[0, y] = true;
                    }
                    if (!Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }
                }

                while (myStackSize > 0)
                {
                    point = myStack[--myStackSize];
                    x = point.X;
                    y = point.Y;

                    //左方向
                    searchTo = x - 1;
                    if (searchTo < width && !Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }

                    //下方向
                    searchTo = y + 1;
                    if (searchTo < height && !Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }

                    //右方向
                    searchTo = x + 1;
                    if (searchTo < width && !Checker[searchTo, y])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(searchTo, y);
                        Checker[searchTo, y] = true;
                    }

                    //上方向
                    searchTo = y - 1;
                    if (searchTo < height && !Checker[x, searchTo])
                    {
                        myStack[myStackSize++] = new MCTProcon29Protocol.Point(x, searchTo);
                        Checker[x, searchTo] = true;
                    }
                }
            }
        }

    }
}
