#include "Game.h"
#include "stdafx.h"

void Game::init(int seed) {
	int y, x, i;

	srand(seed);
	h = 6 + rand() % 7;
	w = 6 + rand() % 7;
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2) scoreMap[y][x] = rand() % 16 + 1;
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2) if (rand() % 100 < 20) scoreMap[y][x] = -(rand() % 17);
	rep(y, h) {
		rep(x, w) {
			int fromY = (y >= (h + 1) / 2) ? h - 1 - y : y;
			int fromX = (x >= (w + 1) / 2) ? w - 1 - x : x;
			scoreMap[y][x] = scoreMap[fromY][fromX];
		}
	}
	rep(i, 2) rep(y, h) rep(x, w) isJinti[i][y][x] = false;

	int startY = rand() % (h / 2);
	int startX = rand() % (w / 2);
	player[0] = Player(startY, startX, 0);
	player[1] = Player(h - 1 - startY, w - 1 - startX, 0);
	player[2] = Player(startY, w - 1 - startX, 1);
	player[3] = Player(h - 1 - startY, startX, 1);
	nowTurn = 0;
	allTurn = 100;
	rep(i, 4) isJinti[player[i].teamId][player[i].y][player[i].x] = true;
}

void Game::draw() {
	{
		int ty = 50, tx = 450-20*w, cellSize = 40;
		int y, x, i;

		//陣地
		rep(y, h) {
			rep(x, w) {
				int ly = ty + cellSize * y;
				int lx = tx + cellSize * x;
				int ry = ly + cellSize;
				int rx = lx + cellSize;
				int color;
				if (isJinti[0][y][x]) color = GetColor(0, 255, 255);
				else if (isJinti[1][y][x]) color = GetColor(255, 200, 0);
				else color = GetColor(255, 255, 255);
				DrawBox(lx, ly, rx, ry, color, TRUE);
			}
		}

		//グリッド
		rep(y, h + 1) DrawLine(tx, ty + cellSize * y, tx + cellSize * w, ty + cellSize * y, 0, 2);
		rep(x, w + 1) DrawLine(tx + cellSize * x, ty, tx + cellSize * x, ty + cellSize * h, 0, 2);

		//数字
		rep(y, h) {
			rep(x, w) {
				int ly = ty + cellSize * y + cellSize / 4;
				int lx = tx + cellSize * x + cellSize / 5;
				DrawFormatString(lx, ly, 0, "%d", scoreMap[y][x]);
			}
		}

		//プレイヤ
		rep(i, 4) {
			int cy = ty + cellSize / 2 + cellSize * player[i].y;
			int cx = tx + cellSize / 2 + cellSize * player[i].x;
			int color = (player[i].teamId == 0) ? GetColor(0, 0, 255) : GetColor(255, 0, 0);
			int r = (cellSize - 2) / 2;
			DrawCircle(cx, cy, r, color, FALSE, 2);
		}
	}
}