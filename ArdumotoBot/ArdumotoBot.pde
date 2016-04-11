  /*  
	sketch Ardumoto.Bot.pde
	created 19/03/2011 14:36:28
  */

  /* add declarations below here */

#include "MotorControl.h"

#define BUFFSIZ 90 
char buffer[BUFFSIZ];
int buffidx;

MotorControl _motor(3,11,12,13);
void setup()
{
	pinMode(13, OUTPUT);
	digitalWrite(13, HIGH);   
	delay(500);              
	digitalWrite(13, LOW);    
	Serial.begin(9600);
	buffidx = 0;
}

void subString(char * fullstring,int from, int count , char *amount) {
	int k;
	int cnt = 0;
	for (k = from; k <= from+count-1; k++ ) {
		amount[cnt++] = fullstring[k];
	}
}

int subStringToInt(char * fullstring, int from, int count ) {
	char amount[count];
	subString(fullstring,from,count,amount);
	return atoi(amount);
}

void loop()
{

	if (Serial.available() > 0) {
		// read the incoming byte:
		char incomingByte = Serial.read();
		if (incomingByte == '?')
		{
			buffidx = 0;
		}
		else if (incomingByte == ';')
		{
			char fullstring[buffidx];
			strncpy(fullstring,buffer,buffidx);
			//Serial.print("Read [");
			//Serial.print(buffidx);
			//Serial.print("]:");
			//Serial.println(fullstring);
			int posi = strchr(fullstring,'D')-fullstring+1;
			if (fullstring[0] == 'S' && posi > 0) {
				int speed = subStringToInt(fullstring,1,posi-2);
				int direction = subStringToInt(fullstring,posi,buffidx-posi);
				_motor.setSpeed(speed);
				_motor.setDirection(direction);
				Serial.println(fullstring);
				Serial.println("ok");
			}
			buffidx = 0;
		}
		else if ((buffidx == BUFFSIZ-1)) {
			buffidx = 0;
			buffer[buffidx++]= incomingByte;
			Serial.println("HUH");
		}
		else {
			buffer[buffidx++]= incomingByte;
		}
	}
}

