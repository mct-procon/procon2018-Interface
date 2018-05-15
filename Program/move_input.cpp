#define _USE_MATH_DEFINES // pi用
#include <cmath>
#include "move.hpp"
#include "keyboard.hpp"

void SetDir(Move *m, DIRECTION value) {
	m->dir = value;
	if (m->action == NONE) m->action = MOVE;
}

void moveInput(Move moves[]) {
	for (int i = 0; i < 2; i++) {
		//エージェント1
		int pov = gamePadGet(i).POV[0];
		//入力されていたとき
		if (pov != -1) {
			if (moves[i * 2].action == NONE) moves[i * 2].action = MOVE;
			moves[i * 2].dir = (DIRECTION)(pov / 4500);
		}
		if (gamePadButtonGet(i,4)==1) {
			moves[i * 2].action = (moves[i * 2].action != MOVE ? MOVE : ERASE);
		}
		//エージェント2
		int x = gamePadGet(i).X;
		int y = gamePadGet(i).Y;
		if (x != 0 || y != 0) {
			if (moves[i * 2 + 1].action == NONE) moves[i * 2 + 1].action = MOVE;
			//383=360+22.5の四捨五入 スティックの入力を調整するため
			int padAngle = ((int)(std::atan2(y, x) *
				180 / M_PI + 90) + 383) % 360;
			int dir = padAngle / 45;
			moves[i * 2 + 1].dir = (DIRECTION)dir;
		}
		if (gamePadButtonGet(i, 5) == 1) {
			moves[i * 2 + 1].action = (
				moves[i * 2 + 1].action != MOVE ? MOVE : ERASE);
		}
	}
	/*for (int i = 1; i < 2; i++) {
		//エージェント1
		if (keyboardGet(KEY_INPUT_UP) > 0)
			SetDir(&moves[i * 2], U);
		else if (keyboardGet(KEY_INPUT_PGUP) > 0)
			SetDir(&moves[i * 2], RU);
		else if (keyboardGet(KEY_INPUT_RIGHT) > 0)
			SetDir(&moves[i * 2], R);
		else if (keyboardGet(KEY_INPUT_PGDN) > 0)
			SetDir(&moves[i * 2], RD);
		else if (keyboardGet(KEY_INPUT_DOWN) > 0)
			SetDir(&moves[i * 2], D);
		else if (keyboardGet(KEY_INPUT_END) > 0)
			SetDir(&moves[i * 2], DL);
		else if (keyboardGet(KEY_INPUT_LEFT) > 0)
			SetDir(&moves[i * 2], L);
		else if (keyboardGet(KEY_INPUT_HOME) > 0)
			SetDir(&moves[i * 2], LU);

		if (keyboardGet(KEY_INPUT_INSERT) == 1) {
			moves[i * 2].action = (moves[i * 2].action != MOVE ? MOVE : ERASE);
		}
		//エージェント2
		if (keyboardGet(KEY_INPUT_NUMPAD8) > 0)
			SetDir(&moves[i * 2 + 1], U);
		else if (keyboardGet(KEY_INPUT_NUMPAD9) > 0)
			SetDir(&moves[i * 2 + 1], RU);
		else if (keyboardGet(KEY_INPUT_NUMPAD6) > 0)
			SetDir(&moves[i * 2 + 1], R);
		else if (keyboardGet(KEY_INPUT_NUMPAD3) > 0)
			SetDir(&moves[i * 2 + 1], RD);
		else if (keyboardGet(KEY_INPUT_NUMPAD2) > 0)
			SetDir(&moves[i * 2 + 1], D);
		else if (keyboardGet(KEY_INPUT_NUMPAD1) > 0)
			SetDir(&moves[i * 2 + 1], DL);
		else if (keyboardGet(KEY_INPUT_NUMPAD4) > 0)
			SetDir(&moves[i * 2 + 1], L);
		else if (keyboardGet(KEY_INPUT_NUMPAD8) > 0)
			SetDir(&moves[i * 2 + 1],LU);

		if (keyboardGet(KEY_INPUT_NUMPAD0) == 1) {
			moves[i * 2 + 1].action = (moves[i * 2 + 1].action != MOVE ? MOVE : ERASE);
		}
	}*/

}