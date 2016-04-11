/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : WorldMood.h
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
see Header

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#include "WorldMood.h"
#include "WProgram.h"

#define INVALID_MOOD_VALUE (-1)
//#define DEBUG (1)

WorldMood::WorldMood(HardwareSerial& print,
                     float emotionSmoothingFactor,
                     float moodSmoothingFactor,
                     float moderateMoodThreshold,
                     float extremeMoodThreshold,
                     const float* const worldTemperamentRatios)   
{
  m_printer = &print;

#ifdef DEBUG
  // check inputs
  if (emotionSmoothingFactor < 0.0f ||
      emotionSmoothingFactor > 1.0f)
  {
    m_printer->print("invalid emotionSmoothingFactor");
  }
  if (moodSmoothingFactor < 0.0f ||
      moodSmoothingFactor > 1.0f)
  {
    m_printer->print("invalid moodSmoothingFactor");
  }
#endif // #ifdef DEBUG

  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    m_worldTemperamentRatios[i] = worldTemperamentRatios[i];
    m_worldMoodCounts[i] = INVALID_MOOD_VALUE;
    m_worldMoodRatios[i] = INVALID_MOOD_VALUE;
  }

  m_emotionSmoothingFactor = emotionSmoothingFactor;
  m_moodSmoothingFactor = moodSmoothingFactor;

  m_moderateMoodThreshold = moderateMoodThreshold;
  m_extremeMoodThreshold = extremeMoodThreshold;

  m_worldMood = NUM_MOOD_TYPES;

#ifdef DEBUG
  // debug code - check sum of m_worldTemperamentRatios is 1.
  float sum = 0;
  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    sum += m_worldTemperamentRatios[i];
  }

  if (sum > 1.0f + 1e-4f || sum < 1.0f - 1e-4f)
  {
    m_printer->print("unexpected m_worldTemperamentRatios sum");
  }
#endif // #ifdef DEBUG
}


void WorldMood::RegisterTweets(int moodID, float tweetsPerMinute)
{
  // check input is valid
  if (moodID < 0 ||
      moodID >= NUM_MOOD_TYPES)
  {
#ifdef DEBUG
    m_printer->print("invalid moodID");
#endif // #ifdef DEBUG
    return;
  }

  if (tweetsPerMinute < 0)
  {
#ifdef DEBUG
    m_printer->print("unexpected tweetsPerMinute");
#endif // #ifdef DEBUG
    return;
  }

  // I'm using the tweetsPerMinute to be equivalent to the current emotion.
  // to get the mood, average these potentially noisy and volatile emotions using an exponential moving average

  if (m_worldMoodCounts[moodID] == INVALID_MOOD_VALUE)
  {
    // first time through
    m_worldMoodCounts[moodID] = tweetsPerMinute;
  }
  else
  {
    const float a = m_emotionSmoothingFactor;
    m_worldMoodCounts[moodID] = (m_worldMoodCounts[moodID] * (1.0f - a)) + (tweetsPerMinute * a);
  }
}

MOOD_INTENSITY WorldMood::ComputeCurrentMoodIntensity()
{
    // check input is valid
  if (m_worldMood < 0 ||
      m_worldMood >= NUM_MOOD_TYPES)
  {
#ifdef DEBUG
    m_printer->print("invalid world mood");
#endif //#ifdef DEBUG
    return MILD;
  }

  if (m_worldTemperamentRatios[m_worldMood] < 1e-4f)
  {
#ifdef DEBUG
    m_printer->print("unexpected m_worldTemperamentRatios");
#endif //#ifdef DEBUG
    return EXTREME;
  }

  // get the mood ratio as a percent of the temperament ratio.
  // this will show the mood ratio as a divergence from the norm, and so is a good measure of mood intensity.
  const float percent = m_worldMoodRatios[m_worldMood] / m_worldTemperamentRatios[m_worldMood];

  if (percent > m_extremeMoodThreshold)
  {
    return EXTREME;
  }
  else if (percent > m_moderateMoodThreshold)
  {
    return CONSIDERABLE;
  }
  else
  {
    return MILD;
  }
}

MOOD_TYPE WorldMood::ComputeCurrentMood()
{
  // find the current ratios
  float sum = 0;
  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    sum += m_worldMoodCounts[i];
  }

  if (sum < 1e-4f)
  {
#ifdef DEBUG
    m_printer->print("unexpected total m_worldMoodCounts");
#endif // ifdef DEBUG

    return m_worldMood;
  }

  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    m_worldMoodRatios[i] = m_worldMoodCounts[i] / sum;
  }

  // find the ratio that has increased by the most, as a proportion of its moving average.
  // So that, for example, an increase from 5% to 10% is more significant than an increase from 50% to 55%.

  float maxIncrease = -1.0f;

  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    float difference = m_worldMoodRatios[i] - m_worldTemperamentRatios[i];

    if (m_worldTemperamentRatios[i] < 1e-4f)
    {
#ifdef DEBUG
      m_printer->print("unexpected m_worldTemperamentRatios");
#endif // ifdef DEBUG
      continue;
    }

    difference /= m_worldTemperamentRatios[i];

    if (difference > maxIncrease)
    {
      maxIncrease = difference;
      m_worldMood = (MOOD_TYPE)i; // this is now the most dominant mood of the world!
    }
  }

  // update the world temperament, as an exponential moving average of the mood.
  // this allows the baseline ratios, i.e. world temperament, to change slowly over time.
  // this means, in affect, that the 2nd derivative of the world mood wrt time is part of the current mood calcuation.
  // and so, after a major anger-inducing event, we can see when people start to become less angry.
  sum = 0;
  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    if (m_worldTemperamentRatios[i]  <= 0)
    {
#ifdef DEBUG
      m_printer->print("m_worldTemperamentRatios should be initialised at construction");
#endif // #ifdef DEBUG

      m_worldTemperamentRatios[i] = m_worldMoodRatios[i];
    }
    else
    {
      const float a = m_moodSmoothingFactor;
      m_worldTemperamentRatios[i] = (m_worldTemperamentRatios[i] * (1.0f - a)) + (m_worldMoodRatios[i] * a);
    }

    sum += m_worldTemperamentRatios[i];
  }

  if (sum < 1e-4f)
  {
#ifdef DEBUG
    m_printer->print("unexpected total m_worldTemperamentRatios total");
#endif // #ifdef DEBUG
    return m_worldMood;
  }

  // and finally, renormalise, to keep the sum of the moving average ratios as 1.0f
  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    m_worldTemperamentRatios[i] *= 1.0f / sum;
  
#ifdef DEBUG
    m_printer->print("temperament ratio: ");
    m_printer->println(m_worldTemperamentRatios[i]);
#endif
    
  }

#ifdef DEBUG
  // debug code - check sum is 1.
  sum = 0;
  for (int i = 0; i < NUM_MOOD_TYPES; i++)
  {
    sum += m_worldTemperamentRatios[i];
  }

  if (sum > 1.0f + 1e-4f || sum < 1.0f - 1e-4f)
  {
    m_printer->println("unexpected renormalise result");
  }
#endif // #ifdef DEBUG

  return m_worldMood;
}

