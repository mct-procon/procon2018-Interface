#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <Windows.h>
using namespace std;

void waitKey(const char message[]) {
	cout << "�L�[���͂�" << message << endl;
	getchar();
}

void errorMessage() {
	cout << "�z��O�̒l�����͂���܂���" << endl;
	getchar();//scanf��enter������getcher�ŏ���
	waitKey("�ē���");
}

int main()
{
	cout << "�N���C�A���g" << endl;
	WSADATA wsaData;
	struct sockaddr_in server;
	SOCKET sock;
	char buf[32];

	// winsock2�̏�����
	WSAStartup(MAKEWORD(2, 0), &wsaData);

	// �\�P�b�g�̍쐬
	sock = socket(AF_INET, SOCK_STREAM, 0);

	// �ڑ���w��p�\���̂̏���
	server.sin_family = AF_INET;
	server.sin_port = htons(12345);
	inet_pton(AF_INET, "127.0.0.1", &server.sin_addr.S_un.S_addr);

	// �T�[�o�ɐڑ�
	while (1) {
		int c = connect(sock, (struct sockaddr *)&server, sizeof(server));
		if (c == -1) {
			cout << "error:�T�[�o�[�ɐڑ��ł��܂���B" << endl;
			waitKey("�I��");
			return 1;
		}
		else break;
	}

	cout << "�ڑ��ɐ������܂����B" << endl;
	while (1) {
		int len = recv(sock, buf, sizeof(buf), 0);
		if (len != -1 || len != 0) {
			buf[len] = '\0';
			break;
		}
	}
	waitKey("���s");

	while (1) {
		int eNum, dir, isEraseMine;
		while (1) {
			system("cls");
			cout << "���Ȃ��̓v���C���[" << buf << "�ł��B" << endl;
			cout << "�w������͂��Ă�������\n" << endl;
			cout << "�G�[�W�F���g�ԍ�(1~2):";
			cin >> eNum;
			if (eNum != 1 && eNum != 2) {
				errorMessage();
				continue;
			}
			cout << "����(0~8):";
			cin >> dir;
			if (!(dir >= 0 && dir <= 8)) {
				errorMessage();
				continue;
			}
			cout << "���^�C������(0:���Ȃ�,1:����):";
			cin >> isEraseMine;
			if (isEraseMine != 0 && isEraseMine != 1) {
				errorMessage();
				continue;
			}
			cout << "" << endl;
			break;
		}
		char sendc[12] = {};
		snprintf(sendc, 6, "%d %d %d", eNum, dir, isEraseMine);
		send(sock, sendc, strlen(sendc), 0);

		cout << "���M����܂���" << endl;
		getchar();//scanf��enter������getcher�ŏ���
		waitKey("���s");
	}

	closesocket(sock);

	// winsock2�̏I������
	WSACleanup();

	return 0;
}