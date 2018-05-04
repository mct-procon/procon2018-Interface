#include "game.hpp"
#include "stdafx.hpp"
#include "move.hpp"
#include "moveInput.hpp"

void Game::init() {
	mapinit();
	nowTurn = 0;
	allTurn = 100;
	actionFrameCount = 0;
	timeLimit = 15;
}

void Game::update() {
	actionFrameCount++;
	moveInput(move);
	if (actionFrameCount / 60 == timeLimit) {
		action();
		nowTurn++;
		actionFrameCount = 0;
	}
}

void Game::draw() {
	{
		int ty = 50, tx = 450 - 20 * w, cellSize = 40;
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

void Game::action() {
	int i, j;
	int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
	int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };
	bool isOk[4];

	//行動可能なプレイヤを検索する
	rep(i, 4) {
		isOk[i] = false;
		if (move[i].action == NONE) continue;	//何もしない
		if (move[i].action == MOVE) {
			int next_y = player[i].y + dy[move[i].dir];
			int next_x = player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= h || next_x < 0 || next_x >= w) { continue; }	//範囲外への移動はダメ
			if (isJinti[!player[i].teamId][next_y][next_x]) { continue; }	//相手マスへの移動はダメ
			int cnt_move = 0;
			rep(j, 4) {
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = player[j].y + dy[move[j].dir];
					nx = player[j].x + dx[move[j].dir];
					if (ny < 0 || ny >= h || nx < 0 || nx >= w || isJinti[!player[j].teamId][ny][nx]) {
						ny = player[j].y;
						nx = player[j].x;
					}
				}
				else {
					ny = player[j].y;
					nx = player[j].x;
				}
				if (ny == next_y && nx == next_x) cnt_move++;
			}
			if (cnt_move > 1) continue;	//複数エージェントが同じマスに移動することはできない
			isOk[i] = true; //移動できる
		}
		if (move[i].action == ERASE) {
			int next_y = player[i].y + dy[move[i].dir];
			int next_x = player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= h || next_x < 0 || next_x >= w) { continue; }	//範囲外の除去はダメ
			if (!isJinti[!player[i].teamId][next_y][next_x]) { continue; }	//相手マス以外の除去はダメ
																						//除去しようとしたマスに次のターン相手が移動する or 留まったままでいる場合は、ダメ
			rep(j, 4) {
				if (player[j].teamId == player[i].teamId) continue;
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = player[j].y + dy[move[j].dir];
					nx = player[j].x + dx[move[j].dir];
					if (ny == next_y && nx == next_x) break;
				}
				else {
					ny = player[j].y;
					nx = player[j].x;
					if (ny == next_y && nx == next_x) break;
				}
			}
			isOk[i] = true; //除去できる
		}
	}

	//実際に行動する
	rep(i, 4) {
		if (!isOk[i]) { continue; }

		int next_y = player[i].y + dy[move[i].dir];
		int next_x = player[i].x + dx[move[i].dir];
		if (move[i].action == MOVE) {
			player[i].y = next_y;
			player[i].x = next_x;
			isJinti[player[i].teamId][next_y][next_x] = true;
		}
		if (move[i].action == ERASE) {
			isJinti[!player[i].teamId][next_y][next_x] = false;
		}
	}
}

void Game::mapinit() {
	int y, x, i;

	h = 6 + GetRand(7 - 1);
	w = 6 + GetRand(7 - 1);
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2) scoreMap[y][x] = GetRand(16 - 1) + 1;
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2) if (GetRand(100 - 1) < 20) scoreMap[y][x] = -(GetRand(17 - 1));
	rep(y, h) {
		rep(x, w) {
			int fromY = (y >= (h + 1) / 2) ? h - 1 - y : y;
			int fromX = (x >= (w + 1) / 2) ? w - 1 - x : x;
			scoreMap[y][x] = scoreMap[fromY][fromX];
		}
	}
	rep(i, 2) rep(y, h) rep(x, w) isJinti[i][y][x] = false;
	int startY = GetRand((h / 2) - 1);
	int startX = GetRand((w / 2) - 1);
	player[0] = Player(startY, startX, 0);
	player[1] = Player(h - 1 - startY, w - 1 - startX, 0);
	player[2] = Player(startY, w - 1 - startX, 1);
	player[3] = Player(h - 1 - startY, startX, 1);
	rep(i, 4) isJinti[player[i].teamId][player[i].y][player[i].x] = true;
}