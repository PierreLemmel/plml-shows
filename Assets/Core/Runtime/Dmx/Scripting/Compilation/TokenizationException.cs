using System;

namespace Plml.Dmx.Scripting.Compilation
{
    internal class TokenizationException : Exception
    {
        public TokenizationException(string message) : base(message) { }
    }
}