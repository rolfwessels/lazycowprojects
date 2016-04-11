using System;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Cgol.Console
{
    public partial class Program
    {
        private void RunCommands()
        {
            if (AppParam.Verbose)
            {
                var repository = (Hierarchy)LogManager.GetRepository();
                var appender = new ConsoleAppender();
                appender.Layout =
                    new PatternLayout("%date %-5level  [%ndc] - %message%newline");

                repository.Root.AddAppender(appender);
                repository.Configured = true;
                repository.RaiseConfigurationChanged(EventArgs.Empty);
                appender.Threshold = Level.Debug;
            }

            //MainCodeGoesHere
        }

    }
}