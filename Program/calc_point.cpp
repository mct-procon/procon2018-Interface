#include <cmath>
#include <string>

namespace {
	//0.���T�� 1.�^�C�� 2.�͂܂�Ă��� 3.�͂܂�Ă��Ȃ� 
	int map[12][12];
	//�E �� �� ��
	int dx[4] = { 1,0,-1,0 };
	int dy[4] = { 0,1,0,-1 };
	bool visitFlag[12][12];
	int _h, _w;
}

bool checkInside(int y, int x);

void calcPoint(
	bool isJinti[2][12][12], int scoreMap[12][12], int h, int w,
	int tilePoints[], int areaPoints[]
) {
	for (int i = 0; i < 2; i++) {
		tilePoints[i] = 0;
		areaPoints[i] = 0;
	}
	//�^�C���|�C���g
	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			if (isJinti[0][y][x]) tilePoints[0] += scoreMap[y][x];
			else if (isJinti[1][y][x]) tilePoints[1] += scoreMap[y][x];
		}
	}
	//�̈�|�C���g �[���D��T�����厲��...
	_h = h;
	_w = w;
	for (int i = 0; i < 2; i++) {
		//map�����Z�b�g
		for (int y = 0; y < h; y++)
			for (int x = 0; x < w; x++)
				map[y][x] = isJinti[i][y][x] ? 1 : 0;
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				//���Z�b�g
				memset(visitFlag, false, sizeof(visitFlag));
				//�����̐w�n�Ȃ�X�L�b�v
				if (isJinti[i][y][x]) { continue; }
				if (checkInside(y, x)) {
					areaPoints[i] += std::abs(scoreMap[y][x]);
				}
			}
		}
	}
}

bool checkInside(int y, int x) {
	if (x < 0 || x >= _w || y < 0 || y >= _h) return false;

	if (visitFlag[y][x]) return true;
	visitFlag[y][x] = true;

	if (map[y][x] == 1) return true;

	for (int i = 0; i < 4; i++) {
		if (!checkInside(y + dy[i], x + dx[i]))  return false;
	}
	return true;
}