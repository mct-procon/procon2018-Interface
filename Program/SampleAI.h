#pragma once
#include "Game.h"
#include "Move.h"

class SampleAI {
public:
	//move1 �c status.player[2 * teamId]�̍s�� (��������)
	//move2 �c status.player[2 * teamId + 1]�̍s�� (��������)
	void think(Game status, int teamId, Move &move1, Move &move2);
};