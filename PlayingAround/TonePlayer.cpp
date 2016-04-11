/*
	TonePlayer.cpp created on 12/03/2011 12:03:36
*/

#include "TonePlayer.h"

TonePlayer::TonePlayer(uint8_t ledPin) {
	this->pin=ledPin;
	pinMode(this->pin,OUTPUT);
}


void TonePlayer::playTone(int tone, int duration) {
  for (long i = 0; i < duration * 1000L; i += tone * 2) {
    digitalWrite(this->pin, HIGH);
    delayMicroseconds(tone);
    digitalWrite(this->pin, LOW);
    delayMicroseconds(tone);
  }
}

void TonePlayer::playNote(char note, int duration) {
  char names[] = { 'c', 'd', 'e', 'f', 'g', 'a', 'b', 'C' };
  int tones[] = { 1915, 1700, 1519, 1432, 1275, 1136, 1014, 956 };

  // play the tone corresponding to the note name
  for (int i = 0; i < 8; i++) {
    if (names[i] == note) {
      playTone(tones[i], duration);
    }
  }
}
