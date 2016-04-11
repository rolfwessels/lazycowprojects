using System;

namespace ImdbPopulate.Console
{
    internal class ProgramException : Exception
    {
        public ProgramException()
            : base()
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
}