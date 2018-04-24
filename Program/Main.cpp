#include "Game.h"
#include "SampleAI.h"

void action(Game &game, Move move[4]) {
	int i, j;
	int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
	int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };
	bool isOk[4];

	//�s���\�ȃv���C������������
	rep(i, 4) {
		isOk[i] = false;
		if (move[i].action == NONE) continue;	//�������Ȃ�
		if (move[i].action == MOVE) {
			int next_y = game.player[i].y + dy[move[i].dir];
			int next_x = game.player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= game.h || next_x < 0 || next_x >= game.w) { continue; }	//�͈͊O�ւ̈ړ��̓_��
			if (game.isJinti[!game.player[i].teamId][next_y][next_x]) { continue; }	//����}�X�ւ̈ړ��̓_��
			int cnt_move = 0;
			rep(j, 4) {
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = game.player[j].y + dy[move[j].dir];
					nx = game.player[j].x + dx[move[j].dir];
					if (ny < 0 || ny >= game.h || nx < 0 || nx >= game.w || game.isJinti[!game.player[j].teamId][ny][nx]) {
						ny = game.player[j].y;
						nx = game.player[j].x;
					}
				}
				else {
					ny = game.player[j].y;
					nx = game.player[j].x;
				}
				if (ny == next_y && nx == next_x) cnt_move++;
			}
			if (cnt_move > 1) continue;	//�����G�[�W�F���g�������}�X�Ɉړ����邱�Ƃ͂ł��Ȃ�
			isOk[i] = true; //�ړ��ł���
		}
		if (move[i].action == ERASE) {
			int next_y = game.player[i].y + dy[move[i].dir];
			int next_x = game.player[i].x + dx[move[i].dir];
			if (next_y < 0 || next_y >= game.h || next_x < 0 || next_x >= game.w) { continue; }	//�͈͊O�̏����̓_��
			if (!game.isJinti[!game.player[i].teamId][next_y][next_x]) { continue; }	//����}�X�ȊO�̏����̓_��
			//�������悤�Ƃ����}�X�Ɏ��̃^�[�����肪�ړ����� or ���܂����܂܂ł���ꍇ�́A�_��
			rep(j, 4) {
				if (game.player[j].teamId == game.player[i].teamId) continue;
				int ny, nx;
				if (move[j].action == MOVE) {
					ny = game.player[j].y + dy[move[j].dir];
					nx = game.player[j].x + dx[move[j].dir];
					if (ny == next_y && nx == next_x) break;
				}
				else {
					ny = game.player[j].y;
					nx = game.player[j].x;
					if (ny == next_y && nx == next_x) break;
				}
			}
			isOk[i] = true; //�����ł���
		}
	}

	//���ۂɍs������
	rep(i, 4) {
		if (!isOk[i]) { continue; }

		int next_y = game.player[i].y + dy[move[i].dir];
		int next_x = game.player[i].x + dx[move[i].dir];
		if (move[i].action == MOVE) {
			game.player[i].y = next_y;
			game.player[i].x = next_x;
			game.isJinti[game.player[i].teamId][next_y][next_x] = true;
		}
		if (move[i].action == ERASE) {
			game.isJinti[!game.player[i].teamId][next_y][next_x] = false;
		}
	}
}

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR arg, int) {

	//dxlib�̏�����
	SetGraphMode(900, 666, 32);
	SetBackgroundColor(255, 255, 255);
	ChangeWindowMode(TRUE);
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);
	
	//�Q�[���̏�����
	Game game;
	SampleAI com1, com2;
	int seed = GetRand(114514);
	game.init(seed);

	clock_t startTime = clock();
	int msecPerTurn = 1000;

	while (ScreenFlip() == 0 && ProcessMessage() == 0 && ClearDrawScreen() == 0) {
		game.draw();

		Move move[4];
		com1.think(game, 0, move[0], move[1]);
		com2.think(game, 1, move[2], move[3]);
		if (clock() - startTime >= msecPerTurn && game.nowTurn < game.allTurn) {
			action(game, move);
			game.nowTurn++;
			startTime = clock();
		}
	}

	DxLib_End();
	return 0;
}