#ifndef RunningAverage_h
#define RunningAverage_h
// 
//    FILE: RunningAverage.h
//  AUTHOR: Rob dot Tillaart at gmail dot com
// PURPOSE: RunningAverage library for Arduino
// HISTORY: See RunningAverage.cpp
//
// Released to the public domain
//

#include <stdio.h>

class RunningAverage 
{
	public:
	RunningAverage(int);
	~RunningAverage();
	void clr();
	void add(float);
	float avg();

protected:
	int _size;
	int _cnt;
	int _idx;
	float _sum;
	float * _ar;
};

#endif

