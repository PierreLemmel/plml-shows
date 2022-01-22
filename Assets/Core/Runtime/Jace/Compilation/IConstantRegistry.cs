using System.Collections.Generic;

namespace Plml.Jace.Compilation
{
    internal interface IConstantRegistry : IEnumerable<ConstantInfo>
    {
        ConstantInfo GetConstantInfo(string constantName);
        bool IsConstantName(string constantName);
        void RegisterConstant(string constantName, float value);
        void RegisterConstant(string constantName, float value, bool isOverWritable);
    }
}