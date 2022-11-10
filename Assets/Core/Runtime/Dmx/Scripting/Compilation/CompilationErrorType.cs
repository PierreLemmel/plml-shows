namespace Plml.Dmx.Scripting.Compilation
{
    internal enum CompilationErrorType
    {
        // Tokenization
        InvalidCharacter,
        InvalidNumberFormat,
        InvalidBrackets,
        InvalidArgCount,
        InvalidTokenizationContext,

        // AST
        UnknownVariable,
        UnsupportedToken,
        InvalidContext,
        MissingProperty,
        UnknownOperator,
        NoLeftHandSideForOperator,
        MissingTargetForOperator,
        TypeError,
        InvalidArgumentType,
        InternalSyntaxTreeError,
        InvalidInternalContext,

        // Compilation
        UnsupportedSyntaxNode,
        UnsupportedVariableType,
        UnsupportedConstantType,
        MissingImplicitConversion,
        MissingExplicitConversion,

        // Optimization
        OptimizationError,

        // Type System
        UnsupportedType,
    }
}