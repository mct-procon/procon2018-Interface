#include <WinSock2.h>

void winsockControll() {
	WSADATA wsaData;

	WSAStartup(MAKEWORD(2, 0), &wsaData);

	WSACleanup();
}