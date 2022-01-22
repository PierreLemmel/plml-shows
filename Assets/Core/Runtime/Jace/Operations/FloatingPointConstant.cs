namespace Plml.Jace.Operations
{
    public class FloatingPointConstant : Operation
    {
        public FloatingPointConstant(float value)
            : base(false)
        {
            this.Value = value;
        }

        public float Value { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is FloatingPointConstant other)
                return this.Value.Equals(other.Value);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}