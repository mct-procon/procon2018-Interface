#include "NIIGATA.h"
#include "SampleAI.h"

void action(NIIGATA &test, Move move[4]) {
	int i, j;
	int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
	int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };
	bool isOk[4];

	//行動可能なプレイヤを検索する
	rep(i, 4) {
		isOk[i] = false;
		if (move[i].action == NONE) continue;	//何もしない
		if (move[i].action == MOVE) {
			int next_y = test.player[i].y + dy[move[i].dir];
			int next_x = test.player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= test.h || next_x < 0 || next_x >= test.w) { continue; }	//範囲外への移動はダメ
			if (test.isJinti[!test.player[i].teamId][next_y][next_x]) { continue; }	//相手マスへの移動はダメ
			int cnt_move = 0;
			rep(j, 4) {
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = test.player[j].y + dy[move[j].dir];
					nx = test.player[j].x + dx[move[j].dir];
					if (ny < 0 || ny >= test.h || nx < 0 || nx >= test.w || test.isJinti[!test.player[j].teamId][ny][nx]) {
						ny = test.player[j].y;
						nx = test.player[j].x;
					}
				}
				else {
					ny = test.player[j].y;
					nx = test.player[j].x;
				}
				if (ny == next_y && nx == next_x) cnt_move++;
			}
			if (cnt_move > 1) continue;	//複数エージェントが同じマスに移動することはできない
			isOk[i] = true; //移動できる
		}
		if (move[i].action == ERASE) {
			int next_y = test.player[i].y + dy[move[i].dir];
			int next_x = test.player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= test.h || next_x < 0 || next_x >= test.w) { continue; }	//範囲外の除去はダメ
			if (!test.isJinti[!test.player[i].teamId][next_y][next_x]) { continue; }	//相手マス以外の除去はダメ
			//除去しようとしたマスに次のターン相手が移動する or 留まったままでいる場合は、ダメ
			rep(j, 4) {
				if (test.player[j].teamId == test.player[i].teamId) continue;
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = test.player[j].y + dy[move[j].dir];
					nx = test.player[j].x + dx[move[j].dir];
					if (ny == next_y && nx == next_x) break;
				}
				else {
					ny = test.player[j].y;
					nx = test.player[j].x;
					if (ny == next_y && nx == next_x) break;
				}
			}
			isOk[i] = true; //除去できる
		}
	}

	//実際に行動する
	rep(i, 4) {
		if (!isOk[i]) { continue; }

		int next_y = test.player[i].y + dy[move[i].dir];
		int next_x = test.player[i].x + dx[move[i].dir];
		if (move[i].action == MOVE) {
			test.player[i].y = next_y;
			test.player[i].x = next_x;
			test.isJinti[test.player[i].teamId][next_y][next_x] = true;
		}
		if (move[i].action == ERASE) {
			test.isJinti[!test.player[i].teamId][next_y][next_x] = false;
		}
	}
}

NIIGATA test;
SampleAI com1, com2;

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR arg, int) {
	SetGraphMode(900, 666, 32);
	SetBackgroundColor(255, 255, 255);
	ChangeWindowMode(TRUE);
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);

	test.init(2521);

	clock_t startTime = clock();
	int msecPerTurn = 1000;

	while (ScreenFlip() == 0 && ProcessMessage() == 0 && ClearDrawScreen() == 0) {
		test.draw();

		Move move[4];
		com1.think(test, 0, move[0], move[1]);
		com2.think(test, 1, move[2], move[3]);
		if (clock() - startTime >= msecPerTurn && test.nowTurn < test.allTurn) {
			action(test, move);
			test.nowTurn++;
			startTime = clock();
		}
	}

	DxLib_End();
	return 0;
}