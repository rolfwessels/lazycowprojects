/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : WorldMood.pde
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
Arduino program to compute the current world mood.
An Arduino connects to any wireless network via the WiFly module, repeatedly searches Twitter for tweets with emotional content, 
collates the tweets for each emotion, analyzes the data, and fades or flashes the color of an LED to reflect the current World Mood: 
Red for Anger, Yellow for Happy, Pink for Love, White for Fear, Green for Envy, Orange for Surprise, and Blue for Sadness.

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#include <HtmlParser.h>
#include <TwitterParser.h>
#include <string.h>
#include <WiFly.h>
#include <WorldMood.h>
#include <LED.h>
#include <avr/pgmspace.h>


// LED setup - only some pins provide 8-bit PWM (Pulse-width modulation)
// output with the analogWrite() function. 
// http://www.arduino.cc/en/Main/ArduinoBoardDuemilanove
// PWM: 3,5,6,9,10,11
#define redPin    (5)
#define greenPin  (6)
#define bluePin   (9)

// delay in ms between fade updates
// max fade time = 255 * 15 = 3.825s
#define fadeDelay (15)

// Wifi setup
#define network ("Mln.co.za")
#define password ("Warl0ck01")
#define remoteServer ("twitter.com")

const char* moodNames[NUM_MOOD_TYPES] = {
  "love",
  "joy",
  "surprise",
  "anger",
  "envy",
  "sadness",
  "fear",
};

const char* moodIntensityNames[NUM_MOOD_INTENSITY] = {
  "mild",
  "considerable",
  "extreme",
};

// the long term ratios between tweets with emotional content
// as discovered by using the below search terms over a period of time.
float tempramentRatios[NUM_MOOD_TYPES] = {
  0.13f,
  0.15f,
  0.20f,
  0.14f,
  0.16f,
  0.12f,
  0.10f,
};

// these numbers can be tweaked to get the system to be more or less reactive
// to be more or less susceptible to noise or short term emotional blips, like sport results 
// or bigger events, like world disasters 
#define  emotionSmoothingFactor (0.1f)
#define  moodSmoothingFactor (0.05f)
#define  moderateMoodThreshold (2.0f)
#define  extremeMoodThreshold (4.0f)

// save battery, put the wifly to sleep for this long between searches (in ms)
#define SLEEP_TIME_BETWEEN_SEARCHES (1000 * 5) 

// Store search strings in flash (program) memory instead of SRAM.
// http://www.arduino.cc/en/Reference/PROGMEM
// edit TWEETS_PER_PAGE if changing the rpp value
prog_char string_0[] PROGMEM = "GET /search.json?q=\"i+love+you\"+OR+\"i+love+her\"+OR+\"i+love+him\"+OR+\"all+my+love\"+OR+\"i'm+in+love\"+OR+\"i+really+love\"&rpp=30&result_type=recent";
prog_char string_1[] PROGMEM = "GET /search.json?q=\"happiest\"+OR+\"so+happy\"+OR+\"so+excited\"+OR+\"i'm+happy\"+OR+\"woot\"+OR+\"w00t\"&rpp=30&result_type=recent";
prog_char string_2[] PROGMEM = "GET /search.json?q=\"wow\"+OR+\"O_o\"+OR+\"can't+believe\"+OR+\"wtf\"+OR+\"unbelievable\"&rpp=30&result_type=recent";
prog_char string_3[] PROGMEM = "GET /search.json?q=\"i+hate\"+OR+\"really+angry\"+OR+\"i+am+mad\"+OR+\"really+hate\"+OR+\"so+angry\"&rpp=30&result_type=recent";
prog_char string_4[] PROGMEM = "GET /search.json?q=\"i+wish+i\"+OR+\"i'm+envious\"+OR+ \"i'm+jealous\"+OR+\"i+want+to+be\"+OR+\"why+can't+i\"+&rpp=30&result_type=recent";
prog_char string_5[] PROGMEM = "GET /search.json?q=\"i'm+so+sad\"+OR+\"i'm+heartbroken\"+OR+\"i'm+so+upset\"+OR+\"i'm+depressed\"+OR+\"i+can't+stop+crying\"&rpp=30&result_type=recent";
prog_char string_6[] PROGMEM = "GET /search.json?q=\"i'm+so+scared\"+OR+\"i'm+really+scared\"+OR+\"i'm+terrified\"+OR+\"i'm+really+afraid\"+OR+\"so+scared+i\"&rpp=30&result_type=recent";

// be sure to change this if you edit the rpp value above
#define TWEETS_PER_PAGE (30)

PROGMEM const char *searchStrings[] = 	   
{   
  string_0,
  string_1,
  string_2,
  string_3,
  string_4,
  string_5,
  string_6,
};

void setup()
{
  Serial.begin(9600);
  delay(100); 
}

void loop()
{
  // create and initialise the subsystems  
  WiFly wifly(network, password, SLEEP_TIME_BETWEEN_SEARCHES, Serial);
  WorldMood worldMood(Serial, emotionSmoothingFactor, moodSmoothingFactor, moderateMoodThreshold, extremeMoodThreshold, tempramentRatios);
  LED led(Serial, redPin, greenPin, bluePin, fadeDelay);
  TwitterParser twitterSearchParser(Serial, TWEETS_PER_PAGE);

  wifly.Reset();
  
  char searchString[160]; 

  while (true)
  {
    for (int i = 0; i < NUM_MOOD_TYPES; i++)
    {
      twitterSearchParser.Reset();

      // read in new search string to SRAM from flash memory 
      strcpy_P(searchString, (char*)pgm_read_word(&(searchStrings[i]))); 

      bool ok = false;
      int retries = 0;

      // some recovery code if the web request fails
      while (!ok)
      {
        ok = wifly.HttpWebRequest(remoteServer, searchString, &twitterSearchParser);

        if (!ok)
        {
          Serial.println("HttpWebRequest failed");

          retries++;
          if (retries > 3)
          {
            wifly.Reset();
            retries = 0;
          }
        }
      }

      float tweetsPerMinute = twitterSearchParser.GetTweetsPerMinute();

      // debug code
      Serial.println("");
      Serial.print(moodNames[i]);
      Serial.print(": tweets per min = ");
      Serial.println(tweetsPerMinute);

      worldMood.RegisterTweets(i, tweetsPerMinute);
    }

    MOOD_TYPE newMood = worldMood.ComputeCurrentMood();
    MOOD_INTENSITY newMoodIntensity = worldMood.ComputeCurrentMoodIntensity();

    Serial.print("The Mood of the World is ... ");
    Serial.print(moodIntensityNames[(int)newMoodIntensity]);
    Serial.print(" ");
    Serial.println(moodNames[(int)newMood]);

    led.SetColor((int)newMood, (int)newMoodIntensity);

    // save the battery
    wifly.Sleep();

    // wait until it is time for the next update
    delay(SLEEP_TIME_BETWEEN_SEARCHES);

    Serial.println("");
  }
}

