namespace Plml.Jace.Operations
{
    public class Exponentiation : Operation
    {
        public Exponentiation(Operation @base, Operation exponent)
            : base(@base.DependsOnVariables || exponent.DependsOnVariables)
        {
            Base = @base;
            Exponent = exponent;
        }

        public Operation Base { get; internal set; }
        public Operation Exponent { get; internal set; }
    }
}
