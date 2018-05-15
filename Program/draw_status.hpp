#pragma once
#include "game.hpp"
//void drawStatusInit();
void drawStatus(
	int actionFrameCount, int timeLimit,
	int nowTurn, int allTurn, DIRECTION dirs[],
	int tilePoints[],int areaPoints[]
);