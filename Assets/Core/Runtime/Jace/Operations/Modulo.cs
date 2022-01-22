namespace Plml.Jace.Operations
{
    public class Modulo : Operation
    {
        public Modulo(Operation dividend, Operation divisor)
            : base(dividend.DependsOnVariables || divisor.DependsOnVariables)
        {
            this.Dividend = dividend;
            this.Divisor = divisor;
        }

        public Operation Dividend { get; internal set; }
        public Operation Divisor { get; internal set; }
    }
}
