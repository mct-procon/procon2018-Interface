#include "dxlib.h"
#include "game.hpp"
#include "move.hpp"
#include "keyboard.hpp"

void dxlibInit();

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR arg, int) {
	dxlibInit();
	//ÉQÅ[ÉÄÇÃèâä˙âª
	Game game;
	game.init();
	while (ScreenFlip() == 0 && ProcessMessage() == 0 &&
		ClearDrawScreen() == 0 && keyboardUpdate() == 0) {
		game.update();
		if (game.getIsInEnd() == true) {
			break;
		}
		game.draw();
	}

	WSACleanup();
	DxLib_End();
	return 0;
}

//dxlibÇÃèâä˙âª
void dxlibInit() {
	SetGraphMode(800, 640, 32);
	SetBackgroundColor(255, 255, 255);
	SetAlwaysRunFlag(TRUE);
	ChangeWindowMode(TRUE);
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);
}
