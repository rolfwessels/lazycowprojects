

#ifndef _WiFly_h
#define _WiFly_h

#include "HardwareSerial.h"

class HtmlParser;


class WiFly
{
public:
  // public interface
  WiFly(const char* ssid, const char* auth_phrase, int sleepTime, HardwareSerial &print);

  bool Reset();
  bool HttpWebRequest(const char* server, const char* getCommand, HtmlParser* parser);
  void Sleep();

private:

  void SPI_UART_Init();
  bool TestSPI_UART_Bridge();

  void WriteByteToRegister(char address, char data);
  void WriteToWiFly(const char *data);
  void WriteToWiFlyCR(const char *data);
  void WriteToWiFlyCRLF(const char *data);

  char ReadCharFromWiFly(char address);
  int  WriteChunk(const char* data, int length);

  void AutoConnect();
  void FlushRX(void);

  char spi_transfer(volatile char data);
  void select();
  void deselect();

  void EnterCommandMode();
  void ExitCommandMode();
  bool WaitUntilReceived(const char* OKresult, const char* errorResult = 0);


  const char* m_network;
  const char* m_password;
  const int m_sleepTime;

  HardwareSerial* m_printer;
};


#endif // #ifndef _WiFly_h
