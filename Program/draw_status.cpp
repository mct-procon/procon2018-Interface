#include "game.hpp"
#include "DxLib.h"

void drawVector(int x, int y, Move move, int charaNum);

void drawStatus(
	int actionFrameCount, int timeLimit,
	int nowTurn, int allTurn, Move moves[],
	int tilePoints[], int areaPoints[]) {
	int c_black = 0;
	//����
	DrawFormatString(420, 600, c_black, "���͎���:%d/%d"
		, actionFrameCount / 60, timeLimit);
	//�^�[��
	DrawFormatString(420, 614, c_black, "�^�[����:%d/%d"
		, nowTurn, allTurn);
	//�|�C���g
	for (int i = 0; i < 2; i++) {
		DrawFormatString(20 + i * 750, 500, c_black,
			"�^�C���|�C���g:%d\n�̈�|�C���g:%d\n���v:%d\n"
			, tilePoints[i], areaPoints[i], tilePoints[i] + areaPoints[i]);
	}
	//����
	int tx = 100;
	int ty = 100;
	int vectorX[4] = { tx,tx,960 - tx,960 - tx };
	int vectorY[4] = { ty,300 + ty,ty,300 + ty };
	for (int i = 0; i < 4; i++) {
		drawVector(vectorX[i], vectorY[i], moves[i], i);
	}
	//�{�^������
	DrawFormatString(400, 700, c_black, "R�Ń��Z�b�g�AESC�ŏI��");
}

void drawVector(int x, int y, Move move, int charaNum) {
	int c_gray = GetColor(128, 128, 128);
	int c_black = 0;
	int c_blue = GetColor(0, 0, 106);
	int c_red = GetColor(146, 0, 0);
	//���̔��p�`
	DrawCircleAA((float)x, (float)y, 50, 8, c_gray, TRUE);
	DrawCircleAA((float)x, (float)y, 50, 8, c_black, FALSE);
	//�J�[�\��
	int culsorX, culsorY;
	if (move.action == NONE) {
		culsorX = x;
		culsorY = y;
	}
	else {
		int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };
		int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
		int dir = move.dir;
		int culsorRange = (dir % 2 == 0 ? 40 : (int)(40 / 1.41));
		culsorX = dx[dir] * culsorRange + x;
		culsorY = dy[dir] * culsorRange + y;
	}
	int culsorColor = (charaNum < 2 ? c_blue : c_red);
	DrawCircleAA((float)culsorX, (float)culsorY, 16, 32, culsorColor, TRUE);
	//����
	std::string showColor;
	std::string showAction;
	showColor = (charaNum < 2 ? "��" : "��");
	switch (move.action) {
	case NONE:
		showAction = "�◯";
		break;
	case MOVE:
		showAction = "�ړ�";
		break;
	case ERASE:
		showAction = "�^�C������";
		break;
	}
	DrawFormatString(x - 50, y - 70, c_black, "%s%d:%s",
		showColor.c_str(), charaNum % 2 + 1, showAction.c_str());
}