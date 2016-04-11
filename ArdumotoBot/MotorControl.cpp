/*
	MotorControl.c created on 02/04/2011 15:05:17
*/

#include "MotorControl.h"

const int MOTORCONTROL_RANGE = 100;
const int MOTORCONTROL_MOTO_RANGE = 255;

//<<constructor>> setup the pins required for speed and direction
MotorControl::MotorControl(uint8_t pwmL,uint8_t pwmR, uint8_t dirL, uint8_t dirR){
	this->_pwmL = pwmL;
	this->_pwmR = pwmR;
	this->_dirL = dirL;
	this->_dirR = dirR;
	//Set control pins to be outputs
	pinMode(this->_pwmL, OUTPUT);  
	pinMode(this->_pwmR, OUTPUT);
	pinMode(this->_dirL, OUTPUT);
	pinMode(this->_dirR, OUTPUT);
	this->stop();
	this->straight();
}

//<<destructor>>
MotorControl::~MotorControl(){/*nothing to destruct*/}

//setSpeed method
void MotorControl::setSpeed(int speed){
	this->_speed = constrain(speed,MOTORCONTROL_RANGE*-1,MOTORCONTROL_RANGE);
	pushValues();
}

//turn method
void MotorControl::setDirection(int precentage){
	this->_direction = constrain(precentage,MOTORCONTROL_RANGE*-1,MOTORCONTROL_RANGE);
	pushValues();
}

//forwards method
void MotorControl::goForward(int speed){
	this->setSpeed(constrain(speed,0,MOTORCONTROL_RANGE));
}

//backwards method
void MotorControl::goBackward(int speed){
	this->setSpeed(constrain(speed,0,MOTORCONTROL_RANGE)*-1);
}

//turnLeft method
void MotorControl::turnLeft(int precentage){
	this->setDirection(this->_direction - precentage);
}

//turnRight method
void MotorControl::turnRight(int precentage){
	this->setDirection(this->_direction + precentage);
}


//stop method
void MotorControl::stop(){
	this->setSpeed(0);
}

//straight method
void MotorControl::straight(){
	this->setDirection(0);
}

//pushValues method
void MotorControl::pushValues(){
	
	int leftWheel = this->_speed ;
	int rightWheel  = this->_speed ;
	if (this->_direction < 0) {
		leftWheel = leftWheel - (this->_speed * ((float)abs(this->_direction) / (MOTORCONTROL_RANGE/2)));
	}
	else {
		rightWheel = rightWheel - (this->_speed * ((float) abs(this->_direction) / (MOTORCONTROL_RANGE/2)));
	}
	analogWrite(this->_pwmL, map(abs(leftWheel),0,100,0,255));        
	analogWrite(this->_pwmR, map(abs(rightWheel),0,100,0,255));
	digitalWrite(this->_dirL, (leftWheel >= 0)?LOW:HIGH);  
	digitalWrite(this->_dirR, (rightWheel <= 0)?LOW:HIGH);  
}
