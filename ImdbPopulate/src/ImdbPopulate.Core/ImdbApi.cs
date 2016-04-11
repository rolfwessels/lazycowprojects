using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web.Script.Serialization;
using ImdbPopulate.UnitTests;
using log4net;

namespace ImdbPopulate.Core
{
    public class ImdbApi
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string _url;

        public ImdbApi()
        {
            _url = "http://www.imdbapi.com/";
        }

        public Stream Cached(Uri uri)
        {
            string path = Path.Combine(Path.GetTempPath(), uri.GetHashCode() + DateTime.Now.Day + "" + DateTime.Now.Month + ".xml");
            Log.Debug(string.Format("ImdbApi:Cached reading uri({0})", uri));
            if (File.Exists(path))
            {
                Log.Debug("ImdbApi:Cached Using cache");
                return File.OpenRead(path);
            }
            var buf = new byte[10000];
            using (var stream = Get(uri))
            {
                using (var tofile = File.OpenWrite(path))
                {
                    int read;
                    while ((read = stream.Read(buf, 0, buf.Length)) > 0)
                    {
                        tofile.Write(buf, 0, read);
                    }
                }

            }
            return File.OpenRead(path);
        }

        public Stream Get(Uri url)
        {
            
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public bool PopulateRating(Movie movie)
        {
            var ser = new JavaScriptSerializer();
            Uri uri;
            Result result;
            if (!string.IsNullOrEmpty(movie.Year) && movie.Year != "0000")
            {
                uri = new Uri(_url + "?t=" + movie.Name+"&y="+movie.Year);
                result = ser.Deserialize<Result>(new StreamReader(Cached(uri)).ReadToEnd());
                if (result.Response && result.imdbRatingDouble > 0)
                {
                    movie.Imdb = result.imdbRatingDouble;
                    return true;
                }

            }
            uri = new Uri(_url+"?t="+movie.Name);
            result = ser.Deserialize<Result>(new StreamReader(Cached(uri)).ReadToEnd());
            if (result.Response && result.Title.ToUpper() == movie.Name.ToUpper() && result.imdbRatingDouble > 0)
            {
                movie.Imdb = result.imdbRatingDouble;
                if (movie.Year == "0000" || string.IsNullOrEmpty(movie.Year))
                {
                    movie.Year = result.Year;
                }
                return true;
            }
            return false;
        }


        //{"Title":"The Matrix","Year":"1999","Rated":"R","Released":"31 Mar 1999","Runtime":"2 h 16 min","Genre":"Action, Adventure, Sci-Fi","Director":"Andy Wachowski, Lana Wachowski","Writer":"Andy Wachowski, Lana Wachowski","Actors":"Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss, Hugo Weaving","Plot":"A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.","Poster":"http://ia.media-imdb.com/images/M/MV5BMjEzNjg1NTg2NV5BMl5BanBnXkFtZTYwNjY3MzQ5._V1_SX640.jpg","imdbRating":"8.7","imdbVotes":"550,137","imdbID":"tt0133093","Response":"True"}
    }
}