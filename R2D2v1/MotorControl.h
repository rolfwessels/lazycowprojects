/*
	MotorControl.h created on 02/04/2011 15:05:18
*/

#ifndef MotorControl_h
#define MotorControl_h

#include "WProgram.h"

class MotorControl 
{
	public:
		MotorControl(uint8_t pwmL,uint8_t pwmR, uint8_t dirL, uint8_t dirR);
		~MotorControl();
		
		void setSpeed(int speed);
		void setDirection(int precentage);
                int getDirection();

                void setUsesSingleMotor(boolean value);
                boolean   UsesSingleMotor();

		void goForward(int speed);
		void goBackward(int speed);
		void turnLeft(int precentage);
		void turnRight(int precentage);

		void stop();
		void straight();

		void pushValues();
	private:
		uint8_t _pwmL; 
		uint8_t _pwmR; 
		uint8_t _dirL; 
		uint8_t _dirR; 
		int _speed;
		int _direction;
                boolean _usesSingleMotor;
};

#endif
