#pragma once
#include "dxlib.h"
int keyboardUpdate();
int keyboardGet(int);
DINPUT_JOYSTATE gamePadGet(int padNum);
int gamePadButtonGet(int padNum, int buttonNum);
