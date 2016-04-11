int red, green, blue; //red, green and blue values
int RedPin = 5; 
int GreenPin = 6; 
int BluePin = 9; 
int TestPin = 13; 

const int smoothing = 3;
byte readings[3][smoothing];
int index[3];
int total[3];

void setup()
{

  Serial.begin(9600);
  pinMode(RedPin, OUTPUT);  
  pinMode(GreenPin, OUTPUT);  
  pinMode(BluePin, OUTPUT);  
  pinMode(TestPin, OUTPUT);  
  //initial values (no significance)
  int red = 0;
  int blue = 123;
  int green = 0;
  
  testColor(TestPin);
  testColor(TestPin);
  testColor(TestPin);
  
  testColor(RedPin);
  testColor(GreenPin);
  testColor(BluePin);

  reset();
}

void loop()
{
 // if (Serial.available() >= 4) {
    if(Serial.read() == 0xff){
      putValue(arrayloc(RedPin), Serial.read());
      putValue(arrayloc(GreenPin), Serial.read());
      putValue(arrayloc(BluePin), Serial.read());
    }
    else {
      digitalWrite (TestPin, HIGH);
      delay(100);
      digitalWrite (TestPin, LOW);
    }
 // }
  writeAll();
  delay(10);
}

void writeAll() {
  analogWrite (RedPin, map(getValue(arrayloc(RedPin)),0,255,0,255));
  analogWrite (GreenPin,  map(getValue(arrayloc(GreenPin)),0,255,0,200));
  analogWrite (BluePin,  map(getValue(arrayloc(BluePin)),0,255,0,255));
}

void testColor(int pin) {
  digitalWrite (pin, HIGH);
  delay(500);
  digitalWrite (pin, LOW);
}

int arrayloc(int pin) {
  if (pin == RedPin) return 0;
  if (pin == GreenPin) return 1;
  return 2;
}

void reset() {
  for (int i = 0; i < 3; i++) {
    resetReadings(i);
  }

  index[0] = 0;
  index[1] = 0;
  index[2] = 0;

  total[0] = 0;
  total[1] = 0;
  total[2] = 0;
}

void resetReadings(int arrayloc) {
  for (int thisReading = 0; thisReading < smoothing; thisReading++)
    readings[arrayloc][thisReading] = 0;
}

void putValue(byte arrayloc,byte value) {
  total[arrayloc] -=  readings[arrayloc][index[arrayloc]];
  readings[arrayloc][index[arrayloc]] = value;
  total[arrayloc] +=  readings[arrayloc][index[arrayloc]];
  index[arrayloc]++;
  if (index[arrayloc] >= smoothing)
    index[arrayloc] = 0;  
}

byte getValue(byte arrayloc) {
  return  total[arrayloc] / smoothing; 
}





