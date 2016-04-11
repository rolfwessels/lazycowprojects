using Plossum.CommandLine;

namespace MainSolutionTemplate.Console
{
    [CommandLineManager(ApplicationName = "MainSolutionTemplate",
        Copyright = "Copyright (C) Private", EnabledOptionStyles = OptionStyles.Group | OptionStyles.LongUnix
        )]
    [CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.AtLeastOne)]
    [CommandLineOptionGroup("options", Name = "Options")]
    class ProgramParams
    {
        #region Commands

        [CommandLineOption(Name = "h", Aliases = "help",
            Description = "Shows this help text", GroupId = "commands")]
        public bool Help { get; set; }

        [CommandLineOption(Name = "f", Aliases = "folder",
            Description = "Some folder something", GroupId = "commands")]
        public string Folder { get; set; }

        #endregion

        #region Options


        [CommandLineOption(Name = "v", Aliases = "verbose",
            Description = "Produce verbose output", GroupId = "options")]
        public bool Verbose { get; set; }

        #endregion

    }
}