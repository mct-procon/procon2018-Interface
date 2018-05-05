#include "game.hpp"
#include "DxLib.h"

void drawVector(int x, int y, Move move, int charaNum);

void drawStatus(
	int actionFrameCount, int timeLimit,
	int nowTurn, int allTurn, Move moves[],
	int tilePoints[], int areaPoints[]) {
	int c_black = 0;
	//時間
	DrawFormatString(420, 600, c_black, "入力時間:%d/%d"
		, actionFrameCount / 60, timeLimit);
	//ターン
	DrawFormatString(420, 614, c_black, "ターン数:%d/%d"
		, nowTurn, allTurn);
	//ポイント
	for (int i = 0; i < 2; i++) {
		DrawFormatString(20 + i * 750, 500, c_black,
			"タイルポイント:%d\n領域ポイント:%d\n合計:%d\n"
			, tilePoints[i], areaPoints[i], tilePoints[i] + areaPoints[i]);
	}
	//方向
	int tx = 100;
	int ty = 100;
	int vectorX[4] = { tx,tx,960 - tx,960 - tx };
	int vectorY[4] = { ty,300 + ty,ty,300 + ty };
	for (int i = 0; i < 4; i++) {
		drawVector(vectorX[i], vectorY[i], moves[i], i);
	}
	//ボタン説明
	DrawFormatString(400, 700, c_black, "Rでリセット、ESCで終了");
}

void drawVector(int x, int y, Move move, int charaNum) {
	int c_gray = GetColor(128, 128, 128);
	int c_black = 0;
	int c_blue = GetColor(0, 0, 106);
	int c_red = GetColor(146, 0, 0);
	//後ろの八角形
	DrawCircleAA((float)x, (float)y, 50, 8, c_gray, TRUE);
	DrawCircleAA((float)x, (float)y, 50, 8, c_black, FALSE);
	//カーソル
	int culsorX, culsorY;
	if (move.action == NONE) {
		culsorX = x;
		culsorY = y;
	}
	else {
		int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };
		int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
		int dir = move.dir;
		int culsorRange = (dir % 2 == 0 ? 40 : (int)(40 / 1.41));
		culsorX = dx[dir] * culsorRange + x;
		culsorY = dy[dir] * culsorRange + y;
	}
	int culsorColor = (charaNum < 2 ? c_blue : c_red);
	DrawCircleAA((float)culsorX, (float)culsorY, 16, 32, culsorColor, TRUE);
	//もじ
	std::string showColor;
	std::string showAction;
	showColor = (charaNum < 2 ? "青" : "赤");
	switch (move.action) {
	case NONE:
		showAction = "停留";
		break;
	case MOVE:
		showAction = "移動";
		break;
	case ERASE:
		showAction = "タイル除去";
		break;
	}
	DrawFormatString(x - 50, y - 70, c_black, "%s%d:%s",
		showColor.c_str(), charaNum % 2 + 1, showAction.c_str());
}