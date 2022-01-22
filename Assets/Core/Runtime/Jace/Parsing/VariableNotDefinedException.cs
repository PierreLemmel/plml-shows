using System;

namespace Plml.Jace.Parsing
{
    public class VariableNotDefinedException : Exception
    {
        public VariableNotDefinedException(string message)
            : base(message)
        {
        }
    }
}