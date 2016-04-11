using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImdbPopulate.Core;
using NUnit.Framework;


namespace ImdbPopulate.UnitTests
{
	[TestFixture]
	public class ReadFolderInfoTests
	{

		[Test]
        public void Read_ReadsAtLeast_One()
		{
            var readFolderInfo = new ReadFolderInfo(@"d:\Movies\New");
		    var enumerable = readFolderInfo.Read();
		    var movies = enumerable as List<Movie> ?? enumerable.ToList();
		    Assert.IsTrue(movies.Any());
            movies.Each(x=> Console.Out.WriteLine(x.Name));
		}

        [Test]
        public void GetMovieInfo_ReadsAtLeast_One1()
        {
            var movie = ReadFolderInfo.GetMovieInfo(new DirectoryInfo(@"p:\Movies\New\96 Minutes 2011"));
            Assert.AreEqual(movie.Name, "96 Minutes");
            Assert.AreEqual(movie.Year, "2011");
            Assert.AreEqual(movie.Imdb, 0);
        }

        [Test]
        public void GetMovieInfo_ReadsAtLeast_One2()
        {
            var movie = ReadFolderInfo.GetMovieInfo(new DirectoryInfo(@"p:\Movies\New\96 Minutes (2011)"));
            Assert.AreEqual(movie.Name, "96 Minutes");
            Assert.AreEqual(movie.Year, "2011");
            Assert.AreEqual(movie.Imdb, 0);
        }

        [Test]
        public void GetMovieInfo_ReadsAtLeast_One3()
        {
            var movie = ReadFolderInfo.GetMovieInfo(new DirectoryInfo(@"p:\Movies\New\96 Minutes [9.4] (2011)"));
            Assert.AreEqual(movie.Name, "96 Minutes");
            Assert.AreEqual(movie.Year, "2011");
            Assert.AreEqual(movie.Imdb, 9.4);
        }

       
	}

    
}

