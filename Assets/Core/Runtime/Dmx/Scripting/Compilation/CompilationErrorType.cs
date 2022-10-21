namespace Plml.Dmx.Scripting.Compilation
{
    internal enum CompilationErrorType
    {
        // Tokenization
        InvalidCharacter,
        InvalidNumberFormat,

        // AST
        UnknownVariable,

        // Type System
        UnsupportedType,
    }
}