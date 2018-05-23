#include <winsock2.h>
#include <iostream>
#include <string>
#include "DxLib.h"

#define CLIENT_SOCK_MAX 2
using namespace std;

namespace {
	WSADATA wsaData;
	SOCKET listenSock;
	struct sockaddr_in addr;
	struct sockaddr_in client;
	int len;
	SOCKET sock;
	char buf[32];
	fd_set readfds;
	SOCKET commSock[CLIENT_SOCK_MAX];
	//resdata[エージェント番号][0.方向,1.自パネル除去フラグ]
	int resdata[4][2];
	struct timeval tv;
}

void comm(int no);

void winsockinit() {
	WSAStartup(MAKEWORD(2, 0), &wsaData);

	listenSock = socket(AF_INET, SOCK_STREAM, 0);

	addr.sin_family = AF_INET;
	addr.sin_port = htons(12345);
	addr.sin_addr.S_un.S_addr = INADDR_ANY;

	bind(listenSock, (struct sockaddr *)&addr, sizeof(addr));

	listen(listenSock, 5);

	for (int i = 0; i < CLIENT_SOCK_MAX; i++) {
		commSock[i] = SOCKET_ERROR;
	}

	u_long val = 1;
	ioctlsocket(listenSock, FIONBIO, &val);
	long a;
	tv.tv_sec = 0;
	tv.tv_usec = 0;
}

void winsock()
{
	FD_ZERO(&readfds);

	FD_SET(listenSock, &readfds);

	for (int i = 0; i < CLIENT_SOCK_MAX; i++) {
		if (commSock[i] != -1) {
			FD_SET(commSock[i], &readfds);
		}
		u_long val = 1;
		ioctlsocket(commSock[i], FIONBIO, &val);
	}

	select(0, &readfds, NULL, NULL, &tv);

	if (FD_ISSET(listenSock, &readfds)) {
		int s;

		// クライアントからの接続要求受付
		len = sizeof(client);

		printf("クライアントからの接続要求受付\n");
		s = accept(listenSock, (struct sockaddr *)&client, &len);

		int sockNum;
		for (sockNum = 0; sockNum < CLIENT_SOCK_MAX; sockNum++) {
			if (commSock[sockNum] == -1) {
				printf("クライアント[%d]の接続を受付完了\n", sockNum);
				commSock[sockNum] = s;
				//クライアント側にプレイヤー番号を知らせる
				char sendc[12];
				printf("%d", sockNum);
				snprintf(sendc, 2, "%d", sockNum + 1);
				printf("%s", sendc);
				send(s, sendc, strlen(sendc), 0);
				break;
			}
		}
		if (sockNum == CLIENT_SOCK_MAX) {
			//接続中クライアントがいっぱいなので
			//受け付けたがすぐ切断
			closesocket(s);
		}
	}

	for (int i = 0; i < 4; i++) {
		for (int j = 0; j < 2; j++) {
			resdata[i][j] = -1;
		}
	}

	for (int i = 0; i < CLIENT_SOCK_MAX; i++) {
		if (FD_ISSET(commSock[i], &readfds)) {
			comm(i);
		}
	}
}

void comm(int no)
{
	char commBuf[256];
	int commLen;

	commLen = recv(commSock[no], commBuf, sizeof(commBuf), 0);
	if (commLen == 0 || commLen == -1) {
		//切断が発生

		printf("クライアント[%d]が切断\n", no);
		closesocket(commSock[no]);
		commSock[no] = -1;
		return;
	}

	commBuf[commLen] = '\0';

	printfDx("[受信] %dより %s\n", no + 1, commBuf);

	int eNum = commBuf[0] - '0' - 1;
	int dir = commBuf[2] - '0';
	int isEraseMine = commBuf[4] - '0';
	resdata[no * 2 + eNum][0] = dir;
	resdata[no * 2 + eNum][1] = isEraseMine != 0;
}

int getdata(int eNum, int dataNum) {
	return resdata[eNum][dataNum];
}