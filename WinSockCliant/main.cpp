#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <Windows.h>
using namespace std;

void waitKey(const char message[]) {
	cout << "キー入力で" << message << endl;
	getchar();
}

void errorMessage() {
	cout << "想定外の値が入力されました" << endl;
	getchar();//scanfのenterをこのgetcherで消化
	waitKey("再入力");
}

int main()
{
	cout << "クライアント" << endl;
	WSADATA wsaData;
	struct sockaddr_in server;
	SOCKET sock;
	char buf[32];

	// winsock2の初期化
	WSAStartup(MAKEWORD(2, 0), &wsaData);

	// ソケットの作成
	sock = socket(AF_INET, SOCK_STREAM, 0);

	// 接続先指定用構造体の準備
	server.sin_family = AF_INET;
	server.sin_port = htons(12345);
	inet_pton(AF_INET, "127.0.0.1", &server.sin_addr.S_un.S_addr);

	// サーバに接続
	while (1) {
		int c = connect(sock, (struct sockaddr *)&server, sizeof(server));
		if (c == -1) {
			cout << "error:サーバーに接続できません。" << endl;
			waitKey("終了");
			return 1;
		}
		else break;
	}

	cout << "接続に成功しました。" << endl;
	while (1) {
		int len = recv(sock, buf, sizeof(buf), 0);
		if (len != -1 || len != 0) {
			buf[len] = '\0';
			break;
		}
	}
	waitKey("続行");

	while (1) {
		int eNum, dir, isEraseMine;
		while (1) {
			system("cls");
			cout << "あなたはプレイヤー" << buf << "です。" << endl;
			cout << "指示を入力してください\n" << endl;
			cout << "エージェント番号(1~2):";
			cin >> eNum;
			if (eNum != 1 && eNum != 2) {
				errorMessage();
				continue;
			}
			cout << "方向(0~8):";
			cin >> dir;
			if (!(dir >= 0 && dir <= 8)) {
				errorMessage();
				continue;
			}
			cout << "自タイル除去(0:しない,1:する):";
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

		cout << "送信されました" << endl;
		getchar();//scanfのenterをこのgetcherで消化
		waitKey("続行");
	}

	closesocket(sock);

	// winsock2の終了処理
	WSACleanup();

	return 0;
}