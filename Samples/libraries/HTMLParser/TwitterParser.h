/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : TwitterParser.h
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
// Responsible for parsing the HTML from Twitter.com and extracting, and making available, the interesting data.
// The Arduino's ram is limited so the HTML is not stored, but passed to the parser for analysis, character by character
// The TwitterParser finds the 'created at' times of the first and last tweet in the search result
// Then a value of "tweets per minute" is made available via GetTweetsPerMinute.

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#ifndef _TwitterParser_h
#define _TwitterParser_h

#include "HtmlParser.h"
#include "HardwareSerial.h"


#define TWITTER_TRIGGER     ("created_at\":\"")
#define TWITTER_BUFFER_SIZE (14)

class TwitterParser : public HtmlParser
{
public:

  TwitterParser(HardwareSerial& print, int tweetsPerPage);

  // set values to defaults
  void Reset();

  // parse the HTML, one character at a time
  // returns true when parser is finished with the HTML
  virtual bool Parse(char a);

  // get the tweets per minute from the most recent html parse
  float GetTweetsPerMinute();

private:

  struct Time
  {
    int m_hour;
    int m_minute;
    int m_second;
  };

  // helper functions
  long   getTotalMinutesBetweenTimes(const Time& t1, const Time& t2);
  bool  isNumber(char a);
  bool  isTimeFormat(const char* buffer);
  void  setTimeFromBuffer(Time& time, const char* buffer);
  
  Time            m_time1;
  Time            m_time2;
  char            m_buffer[TWITTER_BUFFER_SIZE];
  int             m_counter;
  bool            m_foundTrigger;
  
  int             m_tweetsPerPage;
  HardwareSerial* m_printer;
};

#endif // #ifndef _TwitterParser_h

