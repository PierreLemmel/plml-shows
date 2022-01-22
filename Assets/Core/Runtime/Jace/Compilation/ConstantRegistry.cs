using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Plml.Jace.Compilation
{
    internal class ConstantRegistry : IConstantRegistry
    {
        private readonly Dictionary<string, ConstantInfo> constants;

        public ConstantRegistry()
        {
            this.constants = new Dictionary<string, ConstantInfo>();
        }

        public IEnumerator<ConstantInfo> GetEnumerator()
        {
            return constants.Select(p => p.Value).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ConstantInfo GetConstantInfo(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            return constants.TryGetValue(ConvertConstantName(constantName), out ConstantInfo constantInfo) ? constantInfo : null;
        }

        public bool IsConstantName(string constantName)
        {
            if (string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            return constants.ContainsKey(ConvertConstantName(constantName));
        }

        public void RegisterConstant(string constantName, float value)
        {
            RegisterConstant(constantName, value, true);
        }

        public void RegisterConstant(string constantName, float value, bool isOverWritable)
        {
            if(string.IsNullOrEmpty(constantName))
                throw new ArgumentNullException("constantName");

            constantName = ConvertConstantName(constantName);

            if (constants.ContainsKey(constantName) && !constants[constantName].IsOverWritable)
            {
                string message = $"The constant \"{constantName}\" cannot be overwriten.";
                throw new Exception(message);
            }

            ConstantInfo constantInfo = new ConstantInfo(constantName, value, isOverWritable);

            if (constants.ContainsKey(constantName))
                constants[constantName] = constantInfo;
            else
                constants.Add(constantName, constantInfo);
        }

        private string ConvertConstantName(string constantName)
        {
            return constantName.ToLowerInvariant();
        }
    }
}
