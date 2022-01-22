using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Plml.Jace.Compilation
{
    internal class FunctionRegistry : IFunctionRegistry
    {
        private const string DynamicFuncName = "Plml.Jace.DynamicFunc";

        private readonly Dictionary<string, FunctionInfo> functions;

        public FunctionRegistry()
        {
            this.functions = new Dictionary<string, FunctionInfo>();
        }

        public IEnumerator<FunctionInfo> GetEnumerator()
        {
            return functions.Select(p => p.Value).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public FunctionInfo GetFunctionInfo(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            return functions.TryGetValue(ConvertFunctionName(functionName), out FunctionInfo functionInfo) ? functionInfo : null;
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            if (function == null)
                throw new ArgumentNullException("function");

            Type funcType = function.GetType();
            int numberOfParameters = -1;
            
            if (funcType.FullName.StartsWith("System.Func"))
            {
                foreach (Type genericArgument in funcType.GenericTypeArguments)
                    if (genericArgument != typeof(float))
                        throw new ArgumentException("Only float are supported as function arguments.", "function");

                numberOfParameters = function.GetMethodInfo().GetParameters().Length;
            }
            else
                throw new ArgumentException("Only System.Func delegates are permitted.", "function");

            functionName = ConvertFunctionName(functionName);

            if (functions.ContainsKey(functionName) && !functions[functionName].IsOverWritable)
            {
                throw new Exception($"The function \"{functionName}\" cannot be overwriten.");
            }

            if (functions.ContainsKey(functionName) && functions[functionName].NumberOfParameters != numberOfParameters)
            {
                throw new Exception("The number of parameters cannot be changed when overwriting a method.");
            }

            FunctionInfo functionInfo = new FunctionInfo(functionName, numberOfParameters, false, function);

            if (functions.ContainsKey(functionName))
                functions[functionName] = functionInfo;
            else
                functions.Add(functionName, functionInfo);
        }

        public bool IsFunctionName(string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName");

            return functions.ContainsKey(ConvertFunctionName(functionName));
        }

        private string ConvertFunctionName(string functionName)
        {
            return functionName.ToLowerInvariant();
        }
    }
}