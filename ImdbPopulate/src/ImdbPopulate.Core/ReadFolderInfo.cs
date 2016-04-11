using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ImdbPopulate.Core
{
    public class ReadFolderInfo
    {
        private readonly string _path;

        public ReadFolderInfo(string path)
        {
            _path = path;
        }

        public IEnumerable<Movie> Read()
        {
            var directoryInfo = new DirectoryInfo(_path);
            if (directoryInfo.Exists)
            {

                foreach (var dir in directoryInfo.GetDirectories())
                {
                    var movieInfo = GetMovieInfo(dir);
                    if (movieInfo.Year != null)
                    {
                        yield return movieInfo;
                    }
                }
            }
        }

        public static Movie GetMovieInfo(DirectoryInfo dir)
        {
            var name = dir.Name;
            var year = GetValue(@"\(([0-9]{4})\)",ref name);
            if (string.IsNullOrEmpty(year))
            {
                year = GetValue(@" ([0-9]{4})$",ref name);
            }

            double imdb;
            if (double.TryParse(GetValue(@"\[([0-9.]+)\]",ref name), out imdb))
            {

            }
            return new Movie() { Year = year, Imdb = imdb, Name = name, Directory = dir };
        }

        private static string GetValue(string pattern, ref string @string)
        {
            var match = Regex.Match(@string, pattern);
            if (match.Success)
            {
                @string = @string.Replace(match.Captures[0].Value, "").Trim();
                return match.Groups[1].Value;
            }
            return null;
        }

      

        public void Save(Movie movie)
        {
            var fullName = movie.Directory.FullName;            
            string newFolder;
            var directoryName = Path.GetDirectoryName(fullName);
            if (directoryName != null)
            {
                if (movie.Imdb > 0)
                {
                    newFolder = Path.Combine(directoryName,
                                             string.Format("{0} ({1}) [{2}]", movie.Name, movie.Year, movie.Imdb));
                    Directory.Move(fullName, newFolder);
                    return;
                }
                newFolder = Path.Combine(directoryName, string.Format("{0} ({1})", movie.Name, movie.Year));
                Directory.Move(fullName, newFolder);
            }
        }
    }
}