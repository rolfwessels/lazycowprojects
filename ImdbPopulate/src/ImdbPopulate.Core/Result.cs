namespace ImdbPopulate.UnitTests
{
    public class Result
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbRating { get; set; }

        public double imdbRatingDouble
        {
            get
            {
                double score;
                if (double.TryParse(imdbRating, out score))
                {

                }

                return score;
            }
        }
        public bool Response { get; set; }

        public override string ToString()
        {
            return string.Format("Title: {0}, Year: {1}, imdbRating: {2}, Response: {3}", Title, Year, imdbRating, Response);
        }
    }
}