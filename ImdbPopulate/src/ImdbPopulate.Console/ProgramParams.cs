using System.Collections.Generic;
using Plossum.CommandLine;

namespace ImdbPopulate.Console
{
    [CommandLineManager(ApplicationName = "Get Imdb scores",
        Copyright = "Copyright (C) LazyProjects", EnabledOptionStyles = OptionStyles.Group | OptionStyles.LongUnix
        )]
    [CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.AtLeastOne)]
    [CommandLineOptionGroup("options", Name = "Options")]
    class ProgramParams
    {
        public ProgramParams()
        {
            
        }

        #region Commands

        [CommandLineOption(Name = "h", Aliases = "help",
            Description = "Shows this help text", GroupId = "commands")]
        public bool Help { get; set; }

        [CommandLineOption(Name = "f", Aliases = "folder",
            Description = "Update scores in folder", GroupId = "commands")]
        public string Folder { get; set; }

        #endregion


        #region Options


        [CommandLineOption(Name = "v", Aliases = "verbose",
            Description = "Produce verbose output", GroupId = "options")]
        public bool Verbose { get; set; }

        #endregion

    }
}