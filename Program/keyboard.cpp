#include"dxlib.h"

namespace {
	int key[256]; // キーが押されているフレーム数を格納する
	DINPUT_JOYSTATE gamePad[2];
	int buttons[2][32];
}

int keyboardUpdate() {
	char getKey[256];
	GetHitKeyStateAll(getKey);
	for (int i = 0; i < 256; i++) {
		//WASD等の時は別の処理(ゲームパッドの反映)をしたいので飛ばす
		if (getKey[i] != 0) {
			key[i]++;
		}
		else {
			key[i] = 0;
		}
	}

	GetJoypadDirectInputState(DX_INPUT_PAD1, &gamePad[0]);
	GetJoypadDirectInputState(DX_INPUT_PAD2, &gamePad[1]);
	for (int i = 0; i < 2; i++) {
		for (int j = 0; j < 32; j++) {
			//WASD等の時は別の処理(ゲームパッドの反映)をしたいので飛ばす
			if (gamePad[i].Buttons[j] != 0) {
				buttons[i][j]++;
			}
			else {
				buttons[i][j] = 0;
			}
		}
	}
	return 0;
}

int keyboardGet(int KeyCode) {
	return key[KeyCode];
}

DINPUT_JOYSTATE gamePadGet(int padNum) {
	return gamePad[padNum];
}

int gamePadButtonGet(int padNum, int buttonNum) {
	return buttons[padNum][buttonNum];
}
