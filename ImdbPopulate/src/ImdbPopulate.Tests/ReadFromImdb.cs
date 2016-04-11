using System;
using ImdbPopulate.Core;
using NUnit.Framework;


namespace ImdbPopulate.UnitTests
{
    [TestFixture]
    public class ReadFromImdb
    {

        [Test]
        public void Matrix_Matrix_Cool()
        {
            var imdbApi = new ImdbApi();
            var movie = new Movie() { Name = "Matrix", Year = "1999" };
            imdbApi.PopulateRating(movie);
            Console.Out.WriteLine(movie);
            Assert.That(movie.Imdb >= 8.0);
        }

        [Test]
        public void Matrix_TheMatrix_Cool()
        {
            var imdbApi = new ImdbApi();
            var movie = new Movie() { Name = "The Matrix", Year = "" };
            imdbApi.PopulateRating(movie);
            Console.Out.WriteLine(movie);
            Assert.That(movie.Imdb >= 8.0);
        }
        
    }
}