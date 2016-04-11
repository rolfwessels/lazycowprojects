using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ImdbPopulate.Core;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Plossum.CommandLine;

namespace ImdbPopulate.Console
{
    public class Program
	{
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		[STAThread]
        private static int Main(string[] args)
        {
            string log4netFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                              "loggingSettings.xml");
            XmlConfigurator.Configure(new FileInfo(log4netFile));
            var programParams = new ProgramParams();
            var parser = new CommandLineParser(programParams);
            parser.Parse();

            if (programParams.Help)
            {
                System.Console.WriteLine(parser.UsageInfo.ToString(78, false));
            }
            else if (parser.HasErrors)
            {
                System.Console.WriteLine(parser.UsageInfo.ToString(78, true));
                return -1;
            }

            try
            {
                new Program(programParams);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                System.Console.Out.WriteLine("ERROR: " + e.Message);
                return -1;
            }
            return 0;
        }


        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ProgramParams AppParam { get; set; }

        private Program(ProgramParams appParams)
        {
            AppParam = appParams;
            Initialize();
        }

        private void Initialize()
        {
            if (AppParam.Verbose)
            {
                var repository = (Hierarchy) LogManager.GetRepository();
                var appender = new ConsoleAppender();
                appender.Layout =
                    new PatternLayout("%date %-5level  [%ndc] - %message%newline");

                repository.Root.AddAppender(appender);
                repository.Configured = true;
                repository.RaiseConfigurationChanged(EventArgs.Empty);
                appender.Threshold = Level.Debug;
            }

            if (!string.IsNullOrEmpty(AppParam.Folder))
            {
                var readFolderInfo = new ReadFolderInfo(AppParam.Folder);
                var imdbApi = new ImdbApi();
                System.Console.Out.WriteLine("Reading folder:" + AppParam.Folder);
                var movies = readFolderInfo.Read().ToList();
                Log.Info(string.Format("Found {0} movies", movies.Count));
                Log.Info(string.Format("Found {0} movies with no imdb scores", movies.Where(x=> x.Imdb <= 0).Count()));
                foreach (var movie in movies.Where(x => x.Imdb <= 0))
                {
                    Log.Info("Looking up: " + movie.Directory.Name);
                    if (imdbApi.PopulateRating(movie))
                    {
                        Log.Info("Imdb Rating found " + movie.Imdb);
                        try
                        {
                            readFolderInfo.Save(movie);
                        }
                        catch (Exception e)
                        {
                            Log.Error(string.Format("Could not rename folder ({0})", e.Message));
                        }
                    }
                    else
                    {
                        Log.Info("Imdb rating not found");
                    }
                }
                
            }


            //MainCodeGoesHere
        }




	}
}

