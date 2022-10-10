using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plml.Dmx.Scripting.Compilation
{
    internal class CompilationException : Exception
    {
        public CompilationErrorType ErrorType { get; }

        public CompilationException(CompilationErrorType errorType, string message) : base(message)
        {
            ErrorType = errorType;
        }
    }
}
