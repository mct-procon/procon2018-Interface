#define _USE_MATH_DEFINES // pi�p
#include <cmath>
#include "move.hpp"
#include "keyboard.hpp"

void moveInput(Move moves[]) {
	for (int i = 0; i < 2; i++) {
		//�G�[�W�F���g1
		int pov = gamePadGet(i).POV[0];
		//���͂���Ă����Ƃ�
		if (pov != -1) {
			if (moves[i * 2].action == NONE) moves[i * 2].action = MOVE;
			moves[i * 2].dir = (DIRECTION)(pov / 4500);
		}
		if (gamePadButtonGet(i,4)==1) {
			moves[i * 2].action = (moves[i * 2].action != MOVE ? MOVE : ERASE);
		}
		//�G�[�W�F���g2
		int x = gamePadGet(i).X;
		int y = gamePadGet(i).Y;
		if (x != 0 || y != 0) {
			if (moves[i * 2 + 1].action == NONE) moves[i * 2 + 1].action = MOVE;
			//383=360+22.5�̎l�̌ܓ� �X�e�B�b�N�̓��͂𒲐����邽��
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
}