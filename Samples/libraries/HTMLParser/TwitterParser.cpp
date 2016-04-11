/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : TwitterParser.cpp
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
see Header

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#include "TwitterParser.h"
#include "WProgram.h"
#include <string.h>


TwitterParser::TwitterParser(HardwareSerial &print, int tweetsPerPage)
{
  m_printer = &print;
  m_tweetsPerPage = tweetsPerPage;
}


void TwitterParser::Reset()
{
  for (int i = 0; i < TWITTER_BUFFER_SIZE; i++)
  {
    m_buffer[i] = '\0';
  }

  m_foundTrigger = false;

  m_time1.m_hour = 0;
  m_time1.m_minute = 0;
  m_time1.m_minute = 0;

  m_time2.m_hour = 0;
  m_time2.m_minute = 0;
  m_time2.m_minute = 0;

  m_counter = 0;
}

bool TwitterParser::Parse(char a)
{
  // move everything in the buffer down a slot
  for (int i = 0; i < TWITTER_BUFFER_SIZE-2; i++)
  {
    m_buffer[i] = m_buffer[i+1];
  }
  
  // and add the new character
  m_buffer[TWITTER_BUFFER_SIZE-2] = a;
  m_buffer[TWITTER_BUFFER_SIZE-1] = '\0';

  if (!m_foundTrigger)
  {
    // keep looking for the 'created_at":"' in the twitter stream
    if (strstr(m_buffer, TWITTER_TRIGGER))
    {
        m_foundTrigger = true;

#ifdef DEBUG  
        m_printer->println("found trigger");
#endif // #ifdef DEBUG

        m_printer->println(" ");

        m_counter++;
    }
  }
  else
  {
    // found 'created_at":"'
    // look for tweet time, XX:XX:XX
    if (isTimeFormat(m_buffer))
    {
#ifdef DEBUG      
      m_printer->println("found time");
#endif // #ifdef DEBUG

      // now found time of tweet, XX:XX:XX
      m_foundTrigger = false;

      if (m_counter == 1)
      {
        setTimeFromBuffer(m_time1, m_buffer);
      }
      else
      {
        setTimeFromBuffer(m_time2, m_buffer);
        if (m_counter >= m_tweetsPerPage - 1)
        {
          return true;
        }
      }
    }
  }

  return false;
}

float TwitterParser::GetTweetsPerMinute()
{
  long minutes = getTotalMinutesBetweenTimes(m_time1, m_time2);

  if (minutes < 1) 
  {

#ifdef DEBUG
    m_printer->println("unexpected number of minutes");
#endif // #ifdef DEBUG

    minutes = 1;
  }

  const float tps = (float)m_counter / (float)minutes;

  return tps;
}


long TwitterParser::getTotalMinutesBetweenTimes(const Time& t1, const Time& t2)
{
  long time1 = 60*60*t1.m_hour + 60*t1.m_minute + t1.m_second;
  const long time2 = 60*60*t2.m_hour + 60*t2.m_minute + t2.m_second;

  if (time1 < time2)
  {
    // assume that t2 is yesterday.
    // add on 1 day to time1 to keep them both measured from 00:00 to yesterday
    time1 += 24*60*60;

#ifdef DEBUG
    m_printer->println("assuming tweet was from yesterday");
#endif // #ifdef DEBUG
  }

  long seconds = time1 - time2;
  return seconds / 60;
}


bool TwitterParser::isNumber(char a)
{
  if ((int)a >= (int)'0' && (int)a <= (int)'9')
  {
    return true;
  }
  return false;
}

bool  TwitterParser::isTimeFormat(const char* buffer)
{
  // test for time format, XX:XX:XX
  if (
      isNumber(buffer[0]) &&
      isNumber(buffer[1]) &&
      buffer[2] == ':' &&
      isNumber(buffer[3]) &&
      isNumber(buffer[4]) &&
      buffer[5] == ':' &&
      isNumber(buffer[6]) &&
      isNumber(buffer[7])
      )
  {
    return true;
  }

  return false;
}


void TwitterParser::setTimeFromBuffer(Time& time, const char* buffer)
{
  time.m_hour   = atoi(&buffer[0]);
  time.m_minute = atoi(&buffer[3]);
  time.m_second = atoi(&buffer[6]);
}

