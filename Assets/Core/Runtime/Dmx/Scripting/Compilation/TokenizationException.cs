namespace Plml.Dmx.Scripting.Compilation
{
    internal class TokenizationException : CompilationException
    {
        public TokenizationException(CompilationErrorType errorType, string message) : base(errorType, message) { }
    }
}