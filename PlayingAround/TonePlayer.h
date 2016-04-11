/*
	TonePlayer.h created on 12/03/2011 12:03:36
*/


#ifndef TONEPLAYER_H
#define TONEPLAYER_H

#include "WProgram.h" 

class TonePlayer{
  public:
    TonePlayer(uint8_t ledPin);
	void playTone(int tone, int duration);
	void playNote(char note, int duration);
  private:
	uint8_t pin;
};

//extern TONEPLAYER DEBUG_TONE;

#endif