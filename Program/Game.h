#pragma once
#include "stdafx.h"

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
	Player player[4];			//player[0], [1] �c �`�[��0, player[2], [3] �c �`�[��1
	int nowTurn, allTurn;

	void init(int seed);

	void draw();
};
