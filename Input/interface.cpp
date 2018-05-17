#include "winsock.hpp"
#include "dxlib.h"
#include "keyboard.hpp"

void controllInterface() {
	winsockControll();
	while (ScreenFlip() == 0 && ProcessMessage() == 0 &&
		ClearDrawScreen() == 0 && keyboardUpdate() == 0) {
	}
}