#pragma once
//U == 0, RU == 1, Åc
enum DIRECTION {
	U,		//y--
	RU,		//y-- x++
	R,		//    x++
	RD,		//y++ x++
	D,		//y++
	DL,		//y++ x--
	L,		//    x--
	LU,		//y-- x--
	NONE,   //à⁄ìÆÇ»Çµ
};

struct MOVE {
	DIRECTION dir;
	bool isEraseMine;
};