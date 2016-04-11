/*
	AverageCalc.h created on 02/04/2011 15:05:18
*/

#ifndef AverageCalc_h
#define AverageCalc_h

#include "WProgram.h"

class AverageCalc 
{
	public:
		AverageCalc(int _factor);
		~AverageCalc();
		int readValue(int value);
	private:
		int _factor;
                boolean _initialized;
                int _pointer;
                int _values[100];
};

#endif
