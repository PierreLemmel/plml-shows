﻿namespace Plml.Jace.Operations
{
    public class NotEqual : Operation
    {
        public NotEqual(Operation argument1, Operation argument2)
            : base(argument1.DependsOnVariables || argument2.DependsOnVariables)
        {
            this.Argument1 = argument1;
            this.Argument2 = argument2;
        }

        public Operation Argument1 { get; internal set; }
        public Operation Argument2 { get; internal set; }
    }
}
