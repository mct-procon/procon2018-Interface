#pragma once
#include "move.hpp"

class Game {
	class Player {
	public:
		int y, x, teamId;	//y�sx��ڂɂ���.
		Player() {}
		Player(int y, int x, int teamId) { this->y = y; this->x = x; this->teamId = teamId; }
	};

	int h, w;	//�t�B�[���h�̑傫���B�ǂ����2�ȏ�
	int scoreMap[12][12];
	bool isJinti[2][12][12];	//isJinti[teamId(0, 1)][y][x] = �`�[��teamId�̐w�n���H
	MOVE moves[4];
	Player players[4];			//players[0], [1] �c �`�[��0, players[2], [3] �c �`�[��1
	int nowTurn, allTurn;
	int actionFrameCount;
	int timeLimit;
	bool isResult;
	bool isInEnd;
	int tilePoints[2];
	int areaPoints[2];
public:
	void init();
	void update();
	bool getIsInEnd();
	void draw();
private:
	void mapinit();
	void action();
};
