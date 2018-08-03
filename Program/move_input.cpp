#define _USE_MATH_DEFINES // pi�p
#include <cmath>
#include "move.hpp"
#include "keyboard.hpp"
#include "winsock_server.hpp"
#include "dxlib.h"

void moveInput(MOVE moves[]) {
	for (int i = 0; i < 4; i++) {
		DIRECTION dir = (DIRECTION)(getdata(i, 0));
		if (dir != -1)
			moves[i].dir = dir;
		if (getdata(i, 1) != -1) {
			moves[i].isEraseMine = ((getdata(i, 1) != 0));
		}
	}
	/*for (int i = 0; i < 2; i++) {
		//�G�[�W�F���g1
		int pov = gamePadGet(i).POV[0];
		//���͂���Ă����Ƃ�
		if (pov != -1) {
			moves[i * 2] = (DIRECTION)(pov / 4500);
		}
		if (gamePadButtonGet(i, 4) == 1) {
			moves[i * 2] = NONE;
		}
		//�G�[�W�F���g2
		int x = gamePadGet(i).X;
		int y = gamePadGet(i).Y;
		if (x != 0 || y != 0) {
			//383=360+22.5�̎l�̌ܓ� �X�e�B�b�N�̓��͂𒲐����邽��
			int padAngle = ((int)(std::atan2(y, x) *
				180 / M_PI + 90) + 383) % 360;
			int dir = padAngle / 45;
			moves[i * 2 + 1] = (DIRECTION)dir;
		}
		if (gamePadButtonGet(i, 5) == 1) {
			moves[i * 2 + 1] = NONE;
		}
	}*/
}