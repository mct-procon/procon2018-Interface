#pragma once
#include "stdafx.h"

class Game {
public:
	class Player {
	public:
		int y, x, teamId;	//y行x列目にいる.
		Player() {}
		Player(int y, int x, int teamId) { this->y = y; this->x = x; this->teamId = teamId; }
	};

	int h, w;	//フィールドの大きさ。どちらも2以上
	int scoreMap[12][12];
	bool isJinti[2][12][12];	//isJinti[teamId(0, 1)][y][x] = チームteamIdの陣地か？
	Player player[4];			//player[0], [1] … チーム0, player[2], [3] … チーム1
	int nowTurn, allTurn;

	void init(int seed);

	void draw();
};
