#define _USE_MATH_DEFINES // pi用
#include <cmath>
#include "move.hpp"
#include "keyboard.hpp"
void moveInput(Move move[]) {
	int x, y;
	for (int i = 0; i < 2; i++) {
		int pov = gamepadGet(0).POV[0];
		//入力されていたとき
		if (pov != -1) {
			move[i * 2].dir = (DIRECTION)(pov / 4500);
		}
		x = gamepadGet(i).X;
		y = gamepadGet(i).Y;
		if (x != 0 || y != 0) {
			//383=360+22.5の四捨五入 スティックの入力を調整するため
			int padAngle = ((int)(std::atan2(y, x) *
				180 / M_PI + 90) + 383) % 360;
			int dir = padAngle / 45;
			move[i * 2 + 1].dir = (DIRECTION)dir;
		}
	}
}