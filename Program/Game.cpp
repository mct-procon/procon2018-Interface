#include "game.hpp"
#include "stdafx.hpp"
#include "move.hpp"
#include "move_input.hpp"
#include "draw_status.hpp"
#include "calc_point.hpp"
#include "keyboard.hpp"
#include <fstream>
#include <string>

using namespace std;

void Game::init() {
	mapinit();
	//drawStatusInit();
	nowTurn = 1;
	actionFrameCount = 0;
	isResult = false;
	isInEnd = false;
	int i;
	rep(i, 4) dirs[i] = NONE;
	ifstream ifs("config.txt");
	string str;
	if (ifs.fail()) {
		printfDx("ファイル読み込みに失敗しました。");
		isInEnd = true;
	}
	getline(ifs, str);
	sscanf_s(str.data(), "%d,%d", &timeLimit, &allTurn);
}

void Game::update() {
	if (keyboardGet(KEY_INPUT_R) == 1) {
		init();
	}
	if (keyboardGet(KEY_INPUT_ESCAPE) == 1) {
		isInEnd = true;
	}
	if (isResult) return;
	actionFrameCount++;
	moveInput(dirs);

	if (actionFrameCount / 60 == timeLimit) {
		action();
		calcPoint(isJinti, scoreMap, h, w, tilePoints, areaPoints);
		if (nowTurn != allTurn) {
			nowTurn++;
			actionFrameCount = 0;
		}
		else {
			isResult = true;
		}
	}
}

bool Game::getIsInEnd() {
	return isInEnd;
}

void Game::draw() {
	int ty = 90, tx = 400 - 20 * w, cellSize = 40;
	int y, x, i;

	//背景
	DrawBox(0, 0, 800, 640, GetColor(208, 208, 208), TRUE);

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
	rep(y, h + 1) DrawLine(
		tx, ty + cellSize * y, tx + cellSize * w, ty + cellSize * y, 0, 2);
	rep(x, w + 1) DrawLine(
		tx + cellSize * x, ty, tx + cellSize * x, ty + cellSize * h, 0, 2);

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
		int cy = ty + cellSize / 2 + cellSize * players[i].y;
		int cx = tx + cellSize / 2 + cellSize * players[i].x;
		int color = (players[i].teamId == 0)
			? GetColor(50, 50, 255) : GetColor(255, 50, 50);
		int r = (cellSize - 4) / 2;
		DrawCircle(cx, cy, r, color, FALSE, 2);
		color = (players[i].teamId == 0)
			? GetColor(0, 0, 128) : GetColor(128, 0, 0);
		DrawFormatString(cx + 10, cy + 4, color, "%d", i % 2 + 1);
	}

	//ステータス
	drawStatus(
		actionFrameCount, timeLimit, nowTurn,
		allTurn, dirs, tilePoints, areaPoints);
}

void Game::mapinit() {
	int y, x, i;

	//h = 6 + GetRand(7 - 1);
	h = 12;
	//w = 6 + GetRand(7 - 1);
	w = 12;
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2)
		scoreMap[y][x] = GetRand(16 - 1) + 1;
	rep(y, (h + 1) / 2) rep(x, (w + 1) / 2)
		if (GetRand(100 - 1) < 20) scoreMap[y][x] = -(GetRand(17 - 1));
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
	players[0] = Player(startY, startX, 0);
	players[1] = Player(h - 1 - startY, w - 1 - startX, 0);
	players[2] = Player(startY, w - 1 - startX, 1);
	players[3] = Player(h - 1 - startY, startX, 1);
	rep(i, 4) isJinti[players[i].teamId][players[i].y][players[i].x] = true;
	calcPoint(isJinti, scoreMap, h, w, tilePoints, areaPoints);
}


void Game::action() {
	int i, j;
	int dy[9] = { -1, -1, 0, 1, 1, 1, 0, -1 ,0 };
	int dx[9] = { 0, 1, 1, 1, 0, -1, -1, -1 ,0 };
	bool isOk[4];

	//行動可能なプレイヤを検索する
	rep(i, 4) {
		isOk[i] = false;
		if (dirs[i] == NONE) continue;	//何もしない
		int next_y = players[i].y + dy[dirs[i]];
		int next_x = players[i].x + dx[dirs[i]];
		if (next_y < 0 || next_y >= h || next_x < 0 || next_x >= w) {
			dirs[i] = NONE;
			continue;	//範囲外へのアクションはダメ
		}
		//アクション先が相手の陣地以外なら移動
		if (!isJinti[!players[i].teamId][next_y][next_x]) {
			bool isMove = true;
			rep(j, 4) {
				if (j == i) continue;
				int ny, nx;
				ny = players[j].y + dy[dirs[j]];
				nx = players[j].x + dx[dirs[j]];
				if (ny < 0 || ny >= h || nx < 0 || nx >= w || isJinti[!players[j].teamId][ny][nx]) {
					ny = players[j].y;
					nx = players[j].x;
				}
				if (ny == next_y && nx == next_x) isMove = false;
			}
			if (!isMove) {
				dirs[i] = NONE;
				continue;	//複数エージェントが同じマスに移動することはできない
			}
			isOk[i] = true; //移動できる
		}
		//除去
		else {
			//除去しようとしたマスに次のターン相手が移動する or 留まったままでいる場合は、ダメ
			bool isErase = true;
			rep(j, 4) {
				if (players[j].teamId == players[i].teamId) continue;
				int ny, nx;
				ny = players[j].y + dy[dirs[j]];
				nx = players[j].x + dx[dirs[j]];
				if (ny == next_y && nx == next_x) isErase = false;
			}
			if (isErase) isOk[i] = true; //除去できる
		}
	}

	//実際に行動する
	rep(i, 4) {
		if (!isOk[i]) { continue; }

		int next_y = players[i].y + dy[dirs[i]];
		int next_x = players[i].x + dx[dirs[i]];
		if (!isJinti[!players[i].teamId][next_y][next_x]) {
			players[i].y = next_y;
			players[i].x = next_x;
			isJinti[players[i].teamId][next_y][next_x] = true;
		}
		else {
			isJinti[!players[i].teamId][next_y][next_x] = false;
		}
		dirs[i] = NONE;
	}
}
