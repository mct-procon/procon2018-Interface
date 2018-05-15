#define _USE_MATH_DEFINES // pi用
#include <cmath>
#include "move.hpp"
#include "keyboard.hpp"

void moveInput(DIRECTION dirs[]) {
	for (int i = 0; i < 2; i++) {
		//エージェント1
		int pov = gamePadGet(i).POV[0];
		//入力されていたとき
		if (pov != -1) {
			dirs[i * 2] = (DIRECTION)(pov / 4500);
		}
		if (gamePadButtonGet(i, 4) == 1) {
			dirs[i * 2] = NONE;
		}
		//エージェント2
		int x = gamePadGet(i).X;
		int y = gamePadGet(i).Y;
		if (x != 0 || y != 0) {
			//383=360+22.5の四捨五入 スティックの入力を調整するため
			int padAngle = ((int)(std::atan2(y, x) *
				180 / M_PI + 90) + 383) % 360;
			int dir = padAngle / 45;
			dirs[i * 2 + 1] = (DIRECTION)dir;
		}
		if (gamePadButtonGet(i, 5) == 1) {
			dirs[i * 2 + 1] = NONE;
		}
	}
}