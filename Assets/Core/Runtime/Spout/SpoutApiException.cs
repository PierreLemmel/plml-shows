using System;
namespace Plml.Spout
{
    internal class SpoutApiException : Exception
    {
        public SpoutApiException(string message) : base(message) { }
    }
}