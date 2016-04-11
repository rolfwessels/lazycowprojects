/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : LED.h
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
// Simple library for setting the colour of an RGB LED
// use SetColor to fade the RGB LED to a colour in COLORID, definition in cpp
// use FlashColor to fade then flash the RGB LED


Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#ifndef _LED_h
#define _LED_h

#include "HardwareSerial.h"

enum COLORID {
  PINK = 0,
  YELLOW,
  ORANGE,
  RED,
  GREEN,
  BLUE,
  WHITE,
  
  // debug
  BLACK,
  MAGENTA,

  NUM_COLORS,
};

class LED
{
public:
  // public interface
  LED(HardwareSerial &print, int redPin, int greenPin, int bluePin, int fadeDelay);

  void SetColor(int colorID, int flashValue);

private:

  void SetColorSlowFlash(int colorID);
  void SetColorFastFlash(int colorID);

  void FadeTo(int newColorID);
  void Flash(int newColorID, int duration);

  int m_currentColorID;
  int m_fadeDelay;
  int m_redPin;
  int m_greenPin;
  int m_bluePin;

  HardwareSerial* m_printer;
};

#endif // #ifndef _LED_h
