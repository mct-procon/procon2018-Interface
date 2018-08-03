#pragma once
//U == 0, RU == 1, �c
enum DIRECTION {
	U,		//y--
	RU,		//y-- x++
	R,		//    x++
	RD,		//y++ x++
	D,		//y++
	DL,		//y++ x--
	L,		//    x--
	LU,		//y-- x--
	NONE,   //�ړ��Ȃ�
};

struct MOVE {
	DIRECTION dir;
	bool isEraseMine;
};