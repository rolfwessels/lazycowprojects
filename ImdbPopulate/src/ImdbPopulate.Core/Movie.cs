using System.IO;

namespace ImdbPopulate.Core
{
    public class Movie
    {
        public string Year { get; set; }

        public double Imdb { get; set; }

        public string Name { get; set; }

        public DirectoryInfo Directory { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Year: {1}, Imdb: {2}", Name, Year, Imdb);
        }
    }
}