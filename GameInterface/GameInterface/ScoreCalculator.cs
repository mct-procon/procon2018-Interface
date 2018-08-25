﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameInterface
{
    static class ScoreCalculator
    {
        private static int[,] areaStateSearchMap; //未探索 -1 外側 0 内側 1 自陣 2
        private static int height;
        private static int width;
        private static readonly int[] DirectionX = new int[] { 1, 0, -1, 0 };
        private static readonly int[] DirectionY = new int[] { 0, 1, 0, -1 };
        private static bool[,] isSearched;
        public static int CalcScore(int playerNum, int height_, int width_, List<Cell>[] cells)
        {
            int score = 0;
            height = height_;
            width = width_;
            var state = playerNum == 0 ? Cell.AreaState.AREA_1P : Cell.AreaState.AREA_2P;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var cellData = cells[i][j];
                    if (cellData.AreaState_ == state)
                        score += cellData.Score;
                }
            }

            areaStateSearchMap = new int[height, width]; //-1.未探索 0.外側 1.内側 2.自陣
                                                         //囲まれた領域のポイント
                                                         //自陣判定を先にしておく
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (state == cells[j][k].AreaState_)
                        areaStateSearchMap[j, k] = 2;
                    else areaStateSearchMap[j, k] = -1;
                }
            }
            //外側、内側の判定をする
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    isSearched = new bool[height, width]; 
                    if (areaStateSearchMap[j, k] != -1||j==3) continue;
                    CheckIsInside(j, k);
                }
            }

            //内側のものは絶対値を加算する
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (areaStateSearchMap[j, k] == 1)
                        score += Math.Abs(cells[j][k].Score);
                }
            }
            return score;
        }

        private static bool CheckIsInside(int y, int x)
        {
            if (areaStateSearchMap[y, x] == 2) return true;
            isSearched[y, x] = true;
            for (int i = 0; i < 4; i++)
            {
                int ny = y + DirectionY[i], nx = x + DirectionX[i];
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) return false;
                if (isSearched[ny, nx]) continue;
                if (!CheckIsInside(ny,nx))
                {
                    return false;
                }
            }
            areaStateSearchMap[y, x] = 1;
            return true;
        }
    }
}