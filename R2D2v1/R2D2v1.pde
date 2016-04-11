#include "MotorControl.h"
#include "AverageCalc.h"

// Pins
const int leftIrPin =0;
const int centerIrPin =1;
const int rightIrPin =2;
const int  buttonPin = 4;
const int redPin =5;
const int greenPin =6;
const int bluePin =9;
MotorControl _motor(3,11,12,13);

// Variables
float leftRead ;
float centerRead ;
float rightRead ;

AverageCalc leftAc = AverageCalc(5);
AverageCalc centerAc = AverageCalc(5);
AverageCalc rightAc = AverageCalc(5);

float tooClose = 250;
int rightDistance = 130;
int allowDifference = 25;   
// Button settings
int buttonPushCounter = 0;   // counter for the number of button presses
int buttonState = 0;         // current state of the button
int lastButtonState = 0;     // previous state of the button




// Setup
void setup() {
  Serial.begin(9600);
  // declare pins
  pinMode(leftIrPin, INPUT);
  pinMode(centerIrPin, INPUT);
  pinMode(rightIrPin, INPUT);
  pinMode(redPin, OUTPUT);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
  pinMode(buttonPin, INPUT);
  _motor.setUsesSingleMotor(true);
  testLights();
}

// Main Loop
void loop() {
  readInput();
  readButton();
  
  if (buttonPushCounter % 2 == 0) {
    _motor.stop();
    _motor.straight();
    echoOutput();
    outputToLed();
  } 
  else {
    outputToMotor();
  }

  delay(5);
}

void outputToMotor() {
  int fullspeed = 255;
  int maxTurn = 100;
  if (leftRead > tooClose && centerRead > tooClose && leftRead > tooClose) {
    _motor.stop();
    _motor.setDirection(0);
  }
  else {
    _motor.setSpeed(fullspeed);  
    if (centerRead < tooClose || rightRead < tooClose || rightRead < tooClose) {
      int difference = rightRead - rightDistance;
      if (abs(difference) > allowDifference) {
        if (difference > 0) {
          _motor.turnLeft(255);
          Serial.println("left");
        }
        else {
          _motor.turnRight(255);
          Serial.println("right");
        }
      }
      else {
         _motor.straight();
         Serial.println("center");
      }
      Serial.println(difference);
      //_motor.setDirection(map(difference,512,-512,-1*maxTurn,maxTurn));
    }
    else {
      Serial.println("reverse");
      _motor.setSpeed(-1 * fullspeed);  
      _motor.turnRight(255);
      delay(1000);
    }
    analogWrite(redPin, map(constrain(_motor.getDirection(),0,100),1,100,0,50)); 
    analogWrite(bluePin, map(constrain(_motor.getDirection()*-1,0,100),1,100,0,50));
  }

}

void outputToLed() {
  byte maxLight = 50;
  analogWrite(redPin, map(constrain(leftRead,150,700),150,700,0,maxLight)); 
  analogWrite(greenPin, map(constrain(centerRead,150,700),150,700,0,maxLight)); 
  analogWrite(bluePin, map(constrain(rightRead,150,700),150,700,0,maxLight));
}

void readInput() {
  leftRead = smooth(analogRead(leftIrPin), 0.6, leftRead);
  centerRead = smooth(analogRead(centerIrPin), 0.6, centerRead);
  rightRead = smooth(analogRead(rightIrPin), 0.6, rightRead);

//  leftRead = leftAc.readValue(analogRead(leftIrPin));
//  centerRead = centerAc.readValue(analogRead(centerIrPin));
//  rightRead = rightAc.readValue(analogRead(rightIrPin));


}

void testLights() {
  testValue(leftRead);
  testValue(centerRead);
  testValue(rightRead);

}
void testValue(float &readValue ) {
  readValue = 255;
  outputToLed();
  delay(500);
  readValue = 0;
}

void echoOutput() {
  Serial.print(leftRead,DEC);
  Serial.print("_");
  Serial.print(centerRead,DEC);
  Serial.print("_");
  Serial.print(rightRead,DEC);
  Serial.println("");
  
  Serial.print(leftRead,DEC);
  Serial.print("_");
  Serial.print(centerRead,DEC);
  Serial.print("_");
  Serial.print(rightRead,DEC);
  Serial.println("");
}

int smooth(int data, float filterVal, float smoothedVal){
  // check to make sure param's are within range
  filterVal = min(.99,max(0,filterVal));
  smoothedVal = (data * (1 - filterVal)) + (smoothedVal  *  filterVal);
  return (int)smoothedVal;
}

void readButton() {
  // read the pushbutton input pin:
  buttonState = digitalRead(buttonPin);
  if (buttonState != lastButtonState) {
    // if the state has changed, increment the counter
    if (buttonState == HIGH) {
      // if the current state is HIGH then the button
      // wend from off to on:
      buttonPushCounter++;
    } 
  }
  // save the current state as the last state, 
  //for next time through the loop
  lastButtonState = buttonState;
}




