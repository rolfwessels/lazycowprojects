/*    
	sketch PlayingAround.pde
	created 12/03/2011 10:49:47
*/
#include <Button.h>
#include <LED.h>
#include <RgbLed.h>
#include <TonePlayer.h>
#include <RunningAverage.h>

const int blueLedPin1 = 5;
const int blueLedPin2 = 6;
const int speakerPin = 3;
const int potPin = 1;
const int thermistorPin = 2;
const int irSharpPin = 3;

LED blueLed1(blueLedPin1);
LED blueLed2(blueLedPin2);

RgbLedCommonCathode rgbLed(9,10,11,true);
TonePlayer speaker(speakerPin);

RunningAverage distanceRA(10); 
RunningAverage thermistorRa(10); 


void setup()
{
	Serial.begin(9600);
	distanceRA.clr();
	Serial.println("Hello Computer1");
}

void loop()
{
    // pot
	int potValue = analogRead(potPin);
	int potMapped = map(constrain(potValue,5,1023),5,1023,0,254);
	// thermistor
	thermistorRa.add(analogRead(thermistorPin));
	int thermistorValue =  map(constrain(thermistorRa.avg(),505,560),505,560,0,254) ;
	//Serial.println(analogRead(thermistorPin));

	// distance
	distanceRA.add(analogRead(irSharpPin));
	
	int distanceColor = map(constrain(distanceRA.avg(),100,650),100,650,0,254);
	
	

	rgbLed.setColor(thermistorValue,distanceColor,potMapped);
	//blueLed2.setValue(mapped);
	if (potValue > 1000)
	speaker.playTone(map(constrain(distanceRA.avg(),100,650),100,650,900,2000),100);

	
	//Serial.print(thermistorValue);
	//Serial.println("C");
	
	delay(50);
 
}

float read_gp2d12_range(byte pin) {
	int tmp;

	tmp = analogRead(pin);
	if (tmp < 3)
		return -1; // invalid value

	return (6787.0 /((float)tmp - 3.0)) - 4.0;
} 


/*

void playAll() {

	int length = 15; // the number of notes
	char notes[] = "ccggaagffeeddc "; // a space represents a rest
	int beats[] = { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 2, 4 };
	int tempo = 300;
	for (int i = 0; i < length; i++) {
		if (notes[i] == ' ') {
		  delay(beats[i] * tempo); // rest
		} else {
		  speaker.playNote(notes[i], beats[i] * tempo);
		}

		// pause between notes
		delay(tempo / 2); 
	  }
}
*/