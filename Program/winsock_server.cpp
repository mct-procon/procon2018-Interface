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
	//resdata[�G�[�W�F���g�ԍ�][0.����,1.���p�l�������t���O]
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

		// �N���C�A���g����̐ڑ��v����t
		len = sizeof(client);

		printf("�N���C�A���g����̐ڑ��v����t\n");
		s = accept(listenSock, (struct sockaddr *)&client, &len);

		int sockNum;
		for (sockNum = 0; sockNum < CLIENT_SOCK_MAX; sockNum++) {
			if (commSock[sockNum] == -1) {
				printf("�N���C�A���g[%d]�̐ڑ�����t����\n", sockNum);
				commSock[sockNum] = s;
				//�N���C�A���g���Ƀv���C���[�ԍ���m�点��
				char sendc[12];
				printf("%d", sockNum);
				snprintf(sendc, 2, "%d", sockNum + 1);
				printf("%s", sendc);
				send(s, sendc, strlen(sendc), 0);
				break;
			}
		}
		if (sockNum == CLIENT_SOCK_MAX) {
			//�ڑ����N���C�A���g�������ς��Ȃ̂�
			//�󂯕t�����������ؒf
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
		//�ؒf������

		printf("�N���C�A���g[%d]���ؒf\n", no);
		closesocket(commSock[no]);
		commSock[no] = -1;
		return;
	}

	commBuf[commLen] = '\0';

	printfDx("[��M] %d��� %s\n", no + 1, commBuf);

	int eNum = commBuf[0] - '0' - 1;
	int dir = commBuf[2] - '0';
	int isEraseMine = commBuf[4] - '0';
	resdata[no * 2 + eNum][0] = dir;
	resdata[no * 2 + eNum][1] = isEraseMine != 0;
}

int getdata(int eNum, int dataNum) {
	return resdata[eNum][dataNum];
}