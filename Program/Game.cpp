#include "dxlib.h"
#include "game.hpp"
#include "move.hpp"
#include "move_input.hpp"
#include "draw_status.hpp"
#include "calc_point.hpp"
#include "keyboard.hpp"
#include "winsock_server.hpp"
#include <fstream>
#include <string>

using namespace std;

void Game::init() {
	winsockinit();
	mapinit();
	//drawStatusInit();
	nowTurn = 1;
	actionFrameCount = 0;
	isResult = false;
	isInEnd = false;
	for (int i = 0; i < 4; i++) {
		moves[i].dir = NONE;
		moves[i].isEraseMine = false;
	}
	ifstream ifs("config.txt");
	string str;
	if (ifs.fail()) {
		printfDx("�t�@�C���ǂݍ��݂Ɏ��s���܂����B");
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
	winsock();
	moveInput(moves);

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

	//�w�i
	DrawBox(0, 0, 800, 640, GetColor(208, 208, 208), TRUE);
	//�w�n
	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
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

	//�O���b�h
	for (int y = 0; y < h + 1; y++) DrawLine(
		tx, ty + cellSize * y, tx + cellSize * w, ty + cellSize * y, 0, 2);
	for (int x = 0; x < w + 1; x++) DrawLine(
		tx + cellSize * x, ty, tx + cellSize * x, ty + cellSize * h, 0, 2);

	//����
	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			int ly = ty + cellSize * y + cellSize / 4;
			int lx = tx + cellSize * x + cellSize / 5;
			DrawFormatString(lx, ly, 0, "%d", scoreMap[y][x]);
		}
	}

	//�v���C��
	for (int i = 0; i < 4; i++) {
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

	//�X�e�[�^�X
	drawStatus(
		actionFrameCount, timeLimit, nowTurn,
		allTurn, moves, tilePoints, areaPoints);
}

void Game::mapinit() {
	do {
		h = 7 + GetRand(5);
		w = 7 + GetRand(5);
	} while (h*w < 80);

	bool isVertical;
	bool isHorizontal;

	do {
		isVertical = GetRand(1);
		isHorizontal = GetRand(1);
	} while (!isVertical && !isHorizontal);
	int _w, _h;
	_w = (isHorizontal) ? (w + 1) / 2 : w;
	_h = (isVertical) ? (h + 1) / 2 : h;
	for (int y = 0; y < _h; y++) for (int x = 0; x < _w; x++) {
		if (GetRand(99) < 10) scoreMap[y][x] = -(GetRand(15) - 1);
		else scoreMap[y][x] = GetRand(16);
	}
	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			int fromY = (y >= _h) ? h - 1 - y : y;
			int fromX = (x >= _w) ? w - 1 - x : x;
			scoreMap[y][x] = scoreMap[fromY][fromX];
		}
	}
	for (int i = 0; i < 2; i++)
		for (int y = 0; y < h; y++)
			for (int x = 0; x < w; x++) isJinti[i][y][x] = false;

	int startY = GetRand((h / 2) - 1);
	int startX = GetRand((w / 2) - 1);
	players[0] = Player(startY, startX, 0);
	players[1] = Player(h - 1 - startY, w - 1 - startX, 0);
	players[2] = Player(startY, w - 1 - startX, 1);
	players[3] = Player(h - 1 - startY, startX, 1);
	for (int i = 0; i < 4; i++)
		isJinti[players[i].teamId][players[i].y][players[i].x] = true;
	calcPoint(isJinti, scoreMap, h, w, tilePoints, areaPoints);
}


void Game::action() {
	/*
	Q103. �����`�[���̃^�C���������̑Ώۂɂ������A���̃^�C��������`�[���̏����̑ΏۂɂȂ��Ă���ꍇ�́A�ǂ��Ȃ�̂ł��傤���H
	A103. �����^�[���ŕ����̃G�[�W�F���g�������}�X���^�C�������Ɏw�肵���ꍇ�C���`�[���̈ӎv�\���͖����ƂȂ�܂��B
	*/
	int dy[9] = { -1, -1, 0, 1, 1, 1, 0, -1 ,0 };
	int dx[9] = { 0, 1, 1, 1, 0, -1, -1, -1 ,0 };
	bool isOk[4];
	bool isMoove[4];

	clsDx();
	//�s���\�ȃv���C������������
	for (int i = 0; i < 4; i++) {
		isOk[i] = false;
		isMoove[i] = false;
		if (moves[i].dir == NONE) continue;	//�������Ȃ�
		int next_y = players[i].y + dy[(int)moves[i].dir];
		int next_x = players[i].x + dx[(int)moves[i].dir];
		if (next_y < 0 || next_y >= h || next_x < 0 || next_x >= w) {
			moves[i].dir = NONE;
			continue;	//�͈͊O�ւ̃A�N�V�����̓_��
		}
		//�A�N�V�����悪����̐w�n�ȊO�Ȃ�ړ�
		if (!(isJinti[!players[i].teamId][next_y][next_x] || moves[i].isEraseMine)) {
			bool isMove = true;
			for (int j = 0; j < 4; j++) {
				if (j == i) continue;
				int ny, nx;
				ny = players[j].y + dy[(int)moves[j].dir];
				nx = players[j].x + dx[(int)moves[j].dir];
				if (ny < 0 || ny >= h || nx < 0 || nx >= w || isJinti[!players[j].teamId][ny][nx]) {
					ny = players[j].y;
					nx = players[j].x;
				}
				if (ny == next_y && nx == next_x) isMove = false;
			}
			if (!isMove) {
				moves[i].dir = NONE;
				continue;	//�����G�[�W�F���g�������}�X�Ɉړ����邱�Ƃ͂ł��Ȃ�
			}
			isOk[i] = true; //�ړ��ł���
			isMoove[i] = true;
		}
		//����
		else {
			//�������悤�Ƃ����}�X�Ɏ��̃^�[�����肪�ړ����� or ���܂����܂܂ł���ꍇ�́A�_��
			bool isErase = true;
			for (int j = 0; j < 4; j++) {
				if (players[j].teamId == players[i].teamId) continue;
				int ny, nx;
				ny = players[j].y + dy[(int)moves[j].dir];
				nx = players[j].x + dx[(int)moves[j].dir];
				if (ny == next_y && nx == next_x) isErase = false;
			}
			if (isErase) isOk[i] = true; //�����ł���
		}
	}

	//���ۂɍs������
	for (int i = 0; i < 4; i++) {
		if (!isOk[i]) { continue; }

		int next_y = players[i].y + dy[(int)moves[i].dir];
		int next_x = players[i].x + dx[(int)moves[i].dir];
		if (isMoove[i]) {
			players[i].y = next_y;
			players[i].x = next_x;
			isJinti[players[i].teamId][next_y][next_x] = true;
		}
		else {
			isJinti[players[i].teamId][next_y][next_x] = false;
			isJinti[!players[i].teamId][next_y][next_x] = false;
		}
	}

	for (int i = 0; i < 4; i++) {
		moves[i].dir = NONE;
		moves[i].isEraseMine = false;
	}
}
