#include "NIIGATA.h"
#include "SampleAI.h"
#include "stdafx.h"

void think(NIIGATA status, int teamId, Move &move1, Move &move2) {
	//move1 = Move(RU, MOVE);
	//move2 = Move(RU, MOVE);
	//return;

	int i, dir;
	int dy[8] = { -1, -1, 0, 1, 1, 1, 0, -1 };
	int dx[8] = { 0, 1, 1, 1, 0, -1, -1, -1 };

	typedef std::tuple<int, int, int> T;

	rep(i, 2) {
		int playerId = 2 * teamId + i;
		NIIGATA::Player &player = status.player[playerId];
		std::vector<T> dirs;

		rep(dir, 8) {
			int ny = player.y + dy[dir];
			int nx = player.x + dx[dir];
			if (ny < 0 || ny >= status.h || nx < 0 || nx >= status.w || status.isJinti[!teamId][ny][nx]) continue;
			int score = (status.isJinti[teamId][ny][nx] ? 0 : status.scoreMap[ny][nx]);
			dirs.push_back(T(score, rand(), dir));
		}
		std::sort(dirs.begin(), dirs.end(), std::greater<T>());

		if (dirs.size() == 0) { continue; }
		if (i == 0) { move1.dir = (DIRECTION)std::get<2>(dirs[0]); move1.action = MOVE; }
		if (i == 1) { move2.dir = (DIRECTION)std::get<2>(dirs[0]); move2.action = MOVE; }
	}
}