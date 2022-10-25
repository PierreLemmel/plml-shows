namespace Plml.Dmx.Scripting.Compilation
{
    internal enum CompilationErrorType
    {
        // Tokenization
        InvalidCharacter,
        InvalidNumberFormat,
        InvalidBrackets,

        // AST
        UnknownVariable,
        UnsupportedToken,
        InvalidContext,
        MissingProperty,
        UnknownOperator,
        NoLeftHandSideForOperator,

        // Type System
        UnsupportedType,
    }
}