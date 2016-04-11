/*
	AverageCalc.c created on 02/04/2011 15:05:17
 */

#include "AverageCalc.h"

//<<constructor>> setup the pins required for speed and direction
AverageCalc::AverageCalc(int factor){
  this->_factor = factor;
  this->_initialized = false;
  //this->_values;
}

//<<destructor>>
AverageCalc::~AverageCalc(){/*nothing to destruct*/
 //  this->_values = null;
}

//setSpeed method
int AverageCalc::readValue(int value) {
  if (!this->_initialized) {
    this->_initialized = true;
    for (int k = 0; k < this->_factor ; k++) {
      this->_values[k] = value;
    }
  }
  else {
    this->_values[this->_pointer] = value;
    this->_pointer = (this->_pointer+1) % this->_factor;
  }
  int total = 0;
  for (int k = 0; k < this->_factor ; k++) {
    total+=this->_values[k];
  }
  
  int maxv = this->_values[0];
  int minv = this->_values[0];
  
  for (int k = 1; k < this->_factor ; k++) {
    maxv = max(maxv,this->_values[k]);
  }
  for (int k = 1; k < this->_factor ; k++) {
    minv = min(maxv,this->_values[k]);
  }
  total -= (maxv + minv);
  
  return  total/(this->_factor-2);  
  
  
}


