﻿namespace Plml.Jace.Operations
{
    /// <summary>
    /// Represents a variable in a mathematical formula.
    /// </summary>
    public class Variable : Operation
    {
        public Variable(string name)
            : base(true)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public override bool Equals(object obj)
        {
            Variable other = obj as Variable;
            if (other != null)
            {
                return this.Name.Equals(other.Name);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
