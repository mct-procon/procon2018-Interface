#include "game.hpp"
#include "DxLib.h"
#include <string>

/*namespace {
	int bigFont;
}

void drawStatusInit() {
	bigFont=CreateFontToHandle(NULL, 32, -1, DX_FONTTYPE_NORMAL);
}*/
void drawVector(int x, int y, DIRECTION dir, int charaNum);

void drawStatus(
	int actionFrameCount, int timeLimit,
	int nowTurn, int allTurn, DIRECTION dirs[],
	int tilePoints[], int areaPoints[]) {
	int c_black = 0;
	//時間
	DrawFormatString(340, 20, c_black, "入力時間:%d/%d"
		, actionFrameCount / 60, timeLimit);
	//ターン
	DrawFormatString(340, 60, c_black, "ターン数:%d/%d"
		, nowTurn, allTurn);
	//ポイント
	for (int i = 0; i < 2; i++) {
		DrawFormatString(10 + i * 620, 10, c_black,
			"タイルポイント\n領域ポイント\n合計");
		DrawFormatString(130 + i * 620, 10, c_black,
			":%d\n:%d\n:%d\n"
			, tilePoints[i], areaPoints[i], tilePoints[i] + areaPoints[i]);
	}
	//方向
	int tx = 80;
	int ty = 200;
	int vectorX[4] = { tx,tx,800 - tx,800 - tx };
	int vectorY[4] = { ty,150 + ty,ty,150 + ty };
	for (int i = 0; i < 4; i++) {
		drawVector(vectorX[i], vectorY[i], dirs[i], i);
	}
	//ボタン説明
	DrawFormatString(314, 620, c_black, "R:リセット,ESC:終了");
}

void drawVector(int x, int y, DIRECTION dir, int charaNum) {
	int c_gray = GetColor(128, 128, 128);
	int c_black = 0;
	int c_blue = GetColor(0, 0, 106);
	int c_red = GetColor(146, 0, 0);
	//後ろの八角形
	DrawCircleAA((float)x, (float)y, 50, 8, c_gray, TRUE);
	DrawCircleAA((float)x, (float)y, 50, 8, c_black, FALSE);
	//カーソル
	int culsorX, culsorY;
	int dx[9] = { 0, 1, 1, 1, 0, -1, -1, -1,0 };
	int dy[9] = { -1, -1, 0, 1, 1, 1, 0, -1,0 };
	int culsorRange = (dir % 2 == 0 ? 40 : (int)(40 / 1.41));
	culsorX = dx[(int)dir] * culsorRange + x;
	culsorY = dy[(int)dir] * culsorRange + y;
	int culsorColor = (charaNum < 2 ? c_blue : c_red);
	DrawCircleAA((float)culsorX, (float)culsorY, 16, 32, culsorColor, TRUE);
	//もじ
	std::string showColor;
	int textColor;
	showColor = (charaNum < 2 ? "青" : "赤");
	if (charaNum < 2) {
		showColor = "青";
		textColor = c_blue;
	}
	else {
		showColor = "赤";
		textColor = c_red;
	}
	DrawFormatString(x - 50, y - 70, textColor, "%s%d",
		showColor.c_str(), charaNum % 2 + 1);
}