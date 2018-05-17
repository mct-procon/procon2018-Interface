#include "dxlib.h"
#include "keyboard.hpp"
#include "interface.hpp"

void dxlibInit();

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR arg, int) {
	dxlibInit();
	//�Q�[���̏�����
	
	controllInterface();
	DxLib_End();
	return 0;
}

//dxlib�̏�����
void dxlibInit() {
	SetGraphMode(800, 640, 32);
	SetBackgroundColor(255, 255, 255);
	ChangeWindowMode(TRUE);
	DxLib_Init();
	SetDrawScreen(DX_SCREEN_BACK);
}
