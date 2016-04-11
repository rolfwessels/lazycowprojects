

const int redPin = 10;  // Analog input pin that the potentiometer is attached to
const int greenPin = 6; // Analog output pin that the LED is attached to
const int bluePin = 9; // Analog output pin that the LED is attached to
int expectedValue = 0;
int expectedValueSent = 100;
int sendDelay = 50;
int defaultDelay = 1;
byte correctCount = 0;
byte inByte;
boolean startSendingValues;
boolean allSent;
unsigned int started ;
byte failCount = 0;


void setup() {
  pinMode(greenPin, OUTPUT); 
  pinMode(redPin, OUTPUT); 
  pinMode(bluePin, OUTPUT); 
  // initialize serial communications at 9600 bps:
  Serial.begin(9600); 
  
  flash(bluePin);
  flash(bluePin);
  flash(greenPin);
  flash(greenPin);
  flash(greenPin);
  
  started = millis();
}

void loop() {
  
 if (Serial.available() > 0) {
    // get incoming byte:
    inByte = Serial.read();
    started = millis();
    
    if (inByte == expectedValue) {
      flash(greenPin);
      expectedValue++;
      correctCount++;
    }
    else {
       flash(redPin);
       flash(greenPin);flash(greenPin);
       expectedValue = inByte+1;
       failCount++;
    }  
    //  Serial.write(inByte);  
  }
  
  //if last signal is recieved or 10 sec expires
  if (!allSent && !startSendingValues && ( expectedValue == (expectedValueSent) || (millis()  - started) > (10*1000) ) ) {

     Serial.write(correctCount);
     startSendingValues = true;
     delay(1000);
  }
  
  
 
  if (startSendingValues && !allSent ) {
    for(byte sendv = 101 ; sendv <= (100+expectedValueSent) ; sendv++) {
       flash(bluePin); 
       Serial.print(sendv,BYTE);
       delay(max(0,sendDelay));
    }
    allSent = true;  
  }
  
  
  delay(defaultDelay);
}

 void flash(int pin) {
    digitalWrite( pin , HIGH);   // set the LED on
    delay(10);              // wait for a second
    digitalWrite(pin, LOW);    // set the LED off
    delay(10);

}
