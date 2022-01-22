using System;

namespace Plml.Jace.Parsing
{
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {
        }
    }
}