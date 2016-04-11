/** 
 * Serial Call-Response 
 * by Tom Igoe. 
 * 
 * Sends a byte out the serial port, and reads 3 bytes in. 
 * Sets foregound color, xpos, and ypos of a circle onstage
 * using the values returned from the serial port. 
 * Thanks to Daniel Shiffman  and Greg Shakar for the improvements.
 * 
 * Note: This sketch assumes that the device on the other end of the serial
 * port is going to send a single byte of value 65 (ASCII A) on startup.
 * The sketch waits for that byte, then sends an ASCII A whenever
 * it wants more data. 
 */


import processing.serial.*;  

int bgcolor;			     // Background color
int fgcolor;			     // Fill color
Serial myPort;                       // The serial port
PFont fontA;

boolean valuesSent = false;
int delayBetweenSending = 100;
int generaldelayBetweenSending = 10;
byte sendValue = 0;
byte recievedValue = -1;
byte expectedValue = 101;
int correctRecievedValue = 0;
byte sendLimit = 7;
int failed;
long start ;
boolean waitForRecieving;
long totalRecieved = 0 ;
void setup() {
  // setup screen
  size(256, 256);  // Stage size
  noStroke();      // No border on the next thing drawn
  bgcolor = #123123;
  fgcolor = #ffffff;
  background(bgcolor);
  fill(fgcolor);
  fontA = loadFont("AgencyFB-Reg-48.vlw");
  textFont(fontA, 32);
  
  // connect to serial
  println(Serial.list());
  String portName = Serial.list()[0];
  println("Connecting to "+portName);
  myPort = new Serial(this, portName, 9600);
  start  = millis();
}

void draw() {
  background(bgcolor);
  fill(fgcolor);
  text("Sent : "+  sendValue, 10, 30);
  text("Recieved Correct : "+  recievedValue, 10, 60);
  text("Arduino Sent Values : "+  correctRecievedValue, 10, 90);
  text("Arduino Failed Values : "+  failed, 10, 120);
  text("Arduino Total : "+  totalRecieved, 10, 150);
  
  
  if (millis() - start > (2*1000))  {
    if (sendValue < sendLimit ) {
      println(">>"+sendValue);
      //myPort.write(sendValue);
      myPort.write(1);
      sendValue++;    
    }
  }
  
  delay(delayBetweenSending);
}

void serialEvent(Serial myPort) {
  int inByte = myPort.read();
  println("<"+inByte);
  if (inByte <= 100) {
    recievedValue = (byte) inByte;
  }
  else if (inByte <= 200) {
      totalRecieved++;
      if (expectedValue == (byte) inByte) {
          expectedValue++;
          correctRecievedValue++;
      }
      else {
         expectedValue =  (byte) (inByte + 1);
         failed++;
      }
  }
  else {
      println("WTF");
  }
}



