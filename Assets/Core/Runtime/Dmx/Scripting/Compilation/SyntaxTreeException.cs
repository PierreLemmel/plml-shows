namespace Plml.Dmx.Scripting.Compilation
{
    internal class SyntaxTreeException : CompilationException
    {
        public SyntaxTreeException(CompilationErrorType errorType, string message) : base(errorType, message) { }
    }
}