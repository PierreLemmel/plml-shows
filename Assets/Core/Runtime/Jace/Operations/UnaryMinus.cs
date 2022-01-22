namespace Plml.Jace.Operations
{
    public class UnaryMinus : Operation
    {
        public UnaryMinus(Operation argument)
            : base(argument.DependsOnVariables)
        {
            this.Argument = argument;
        }

        public Operation Argument { get; internal set; }
    }
}
