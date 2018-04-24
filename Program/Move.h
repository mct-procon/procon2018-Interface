//各プレイヤーが示すコマンド

#pragma once
#include "stdafx.h"

//U == 0, RU == 1, …
enum DIRECTION {
	U,		//y--
	RU,		//y-- x++
	R,		//    x++
	RD,		//y++ x++
	D,		//y++
	DL,		//y++ x--
	L,		//    x--
	LU,		//y-- x--
};

enum ACTION {
	NONE,
	MOVE,
	ERASE,
};

struct Move {
	DIRECTION dir;
	ACTION action;
	Move() {}
	Move(DIRECTION dir, ACTION action) { this->dir = dir; this->action = action; }
};