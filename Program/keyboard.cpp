#include"dxlib.h"

namespace {
	int key[256]; // �L�[��������Ă���t���[�������i�[����
	DINPUT_JOYSTATE gamepad[2];
}

int keyboardUpdate() {
	char getKey[256];
	GetHitKeyStateAll(getKey);
	for (int i = 0; i < 256; i++) {
		//WASD���̎��͕ʂ̏���(�Q�[���p�b�h�̔��f)���������̂Ŕ�΂�
		if (getKey[i] != 0) {
			key[i]++;
		}
		else {
			key[i] = 0;
		}
	}

	GetJoypadDirectInputState(DX_INPUT_PAD1, &gamepad[0]);
	GetJoypadDirectInputState(DX_INPUT_PAD2, &gamepad[1]);

	return 0;
}

int keyboardGet(int KeyCode) {
	return key[KeyCode];
}

DINPUT_JOYSTATE gamepadGet(int padNum) {
	return gamepad[padNum];
}
