using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Plossum.CommandLine;

namespace MvcTemplate.Console
{
    public partial class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Program(ProgramParams appParams)
        {
            AppParam = appParams;
            RunCommands();
        }

        private ProgramParams AppParam { get; set; }

        // ReSharper disable UnusedParameter.Local
        [STAThread]
        private static int Main(string[] args)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            string log4NetFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                              "loggingSettings.xml");
            // ReSharper restore AssignNullToNotNullAttribute
            XmlConfigurator.Configure(new FileInfo(log4NetFile));
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
                Log.Error(e.Message, e);
                System.Console.Out.WriteLine("ERROR: " + e.Message);
                return -1;
            }
            return 0;
        }

        // ReSharper restore UnusedParameter.Local

        #region Nested type: ProgramException

        internal class ProgramException : Exception
        {
            public ProgramException()
            {
            }

            public ProgramException(string message)
                : base(message)
            {
            }

            public ProgramException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        #endregion
    }
}