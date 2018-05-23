#pragma once
#include "move.hpp"

class Game {
	class Player {
	public:
		int y, x, teamId;	//y行x列目にいる.
		Player() {}
		Player(int y, int x, int teamId) { this->y = y; this->x = x; this->teamId = teamId; }
	};

	int h, w;	//フィールドの大きさ。どちらも2以上
	int scoreMap[12][12];
	bool isJinti[2][12][12];	//isJinti[teamId(0, 1)][y][x] = チームteamIdの陣地か？
	MOVE moves[4];
	Player players[4];			//players[0], [1] … チーム0, players[2], [3] … チーム1
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
