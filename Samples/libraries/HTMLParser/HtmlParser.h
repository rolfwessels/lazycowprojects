/* 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

Title : HtmlParser.h
Author : http://www.instructables.com/member/RandomMatrix/

Description : 
// HtmlParser
// Abstract base class
// Inherit from this and pass into WiFly::HttpWebRequest so as to parse the HTML, character by character, as it is streamed.
// Parse should return true when it has no more use for the HTML so the HttpWebRequest can return

Created : April 22 2010
Modified : 

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
*/
#ifndef _HtmlParser_h
#define _HtmlParser_h

class HtmlParser
{
public:
  virtual bool Parse(char a) = 0;

private:
};


#endif // #ifndef _HtmlParser_h

