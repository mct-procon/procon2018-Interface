#pragma once
#include "game.hpp"
//void drawStatusInit();
void drawStatus(
	int actionFrameCount, int timeLimit,
	int nowTurn, int allTurn, MOVE moves[],
	int tilePoints[],int areaPoints[]
);