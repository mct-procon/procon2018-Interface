//各プレイヤーが示すコマンド

#pragma once
#include "stdafx.hpp"

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
	Move() { dir = U; action = NONE; }
};