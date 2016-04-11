/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : WorldMood.h
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
A store for what the World Mood has been in the past and what it is currently.
use RegisterTweets after searching twitter for tweets with emotional content
and register the tweets per second and the particular mood.
Call ComputeCurrentMood to get a normalized and more stable idea of the World Mood.
ComputeCurrentMoodIntensity gives an idea of how strong the current mood is
            

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/

#ifndef _WorldMood_h
#define _WorldMood_h

#include "HardwareSerial.h"

enum MOOD_TYPE {

  LOVE = 0,
  JOY,
  SURPRISE,
  ANGER,
  ENVY,
  SADNESS,
  FEAR,

  NUM_MOOD_TYPES,
};

enum MOOD_INTENSITY {

  MILD = 0,
  CONSIDERABLE,
  EXTREME,

  NUM_MOOD_INTENSITY,
};


class WorldMood
{
public:

  // public interface
  WorldMood( 
      HardwareSerial &print,                  // for debug printing
      float emotionSmoothingFactor,           // The world mood is calculated as an exponential moving average
                                              // of world emotion, with this as the smoothing factor. EMA = (1-alpha)*EMA + alpha*x
      float moodSmoothingFactor,              // As above, but for tracking the world temperament as the EMA of the world mood
                                              // the current mood is calculated by finding the mood type that has most diverged from
                                              // the world temperament.
      float considerableMoodThreshold,        // The world mood is the mood type that makes up the highest percent compared to what is normal.
                                              // when it diverges by this ratio the mood is considered considerable.
      float extremeMoodThreshold,             // Similarly, when it diverges by this much it is considered extreme.
      const float* const worldTemperamentRatios     // the base-line ratios that describe the long term ratios of the world mood types, aka the temperament.
    );

  // inform the World Mood instance of the latest tweets per second for a certain mood.
  void RegisterTweets(int moodID, float tweetsPerSecond);

  // compute the current mood of the world by what the current ratios of each mood compared to a longterm average
  MOOD_TYPE    ComputeCurrentMood();

  // and how much the deviation is from the average, aka, the intensity
  MOOD_INTENSITY  ComputeCurrentMoodIntensity();

private:

  MOOD_TYPE  m_worldMood;

  float m_worldTemperamentRatios[NUM_MOOD_TYPES];
  float m_worldMoodCounts[NUM_MOOD_TYPES];
  float m_worldMoodRatios[NUM_MOOD_TYPES];

  float m_emotionSmoothingFactor;
  float m_moodSmoothingFactor;
  float m_moderateMoodThreshold;
  float m_extremeMoodThreshold;

  HardwareSerial* m_printer;
};

#endif // #ifndef _WorldMood_h

