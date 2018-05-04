#pragma once
#include "stdafx.hpp"
#include "move.hpp"

class Game {
public:
	class Player {
	public:
		int y, x, teamId;	//y�sx��ڂɂ���.
		Player() {}
		Player(int y, int x, int teamId) { this->y = y; this->x = x; this->teamId = teamId; }
	};

	int h, w;	//�t�B�[���h�̑傫���B�ǂ����2�ȏ�
	int scoreMap[12][12];
	bool isJinti[2][12][12];	//isJinti[teamId(0, 1)][y][x] = �`�[��teamId�̐w�n���H
	Move move[4];
	Player player[4];			//player[0], [1] �c �`�[��0, player[2], [3] �c �`�[��1
	int nowTurn, allTurn;
	int actionFrameCount;
	int timeLimit;

	void init();
	void update();
	void draw();
	void action();
	void mapinit();
};
