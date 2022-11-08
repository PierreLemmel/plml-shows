using Plml.Dmx.Scripting.Types;
using System;

namespace Plml.Dmx.Scripting.Compilation
{
    public class LightScriptFunction
    {
        public LightScriptType ReturnType { get; }
        public string Name { get; }
        public LightScriptArgument[] Arguments { get; }
        public bool IsPure { get; }
        public Delegate Function { get; }

        public LightScriptFunction(LightScriptType returnType, string name, bool pure, Delegate function, params LightScriptArgument[] arguments)
        {
            var method = function.Method;
            var delParamaters = method.GetParameters();
            if (delParamaters.Length != arguments.Length)
                throw new LightScriptException("LightScriptFunction and underlying Delegate must have the same count of arguments");

            for (int i = 0; i < arguments.Length; i++)
            {
                Type sysType = delParamaters[i].ParameterType;
                LightScriptType expectedLSType = arguments[i].Type;
                if (!CheckTypeCompatibility(sysType, expectedLSType))
                {
                    throw new LightScriptException($"Invalid type in delegate for function '{name}' in argument {i}: impossible to map system type '{sysType.FullName}' to '{expectedLSType}'");
                }
            }

            if (!CheckTypeCompatibility(method.ReturnType, returnType))
                throw new LightScriptException($"Invalid in delegate for function '{name}' in return type: impossible to map system type '{method.ReturnType}' to '{returnType}'");


            ReturnType = returnType;
            Name = name;
            Arguments = arguments.ShallowCopy();
            IsPure = pure;
            Function = function;
        }

        public LightScriptFunction(LightScriptType returnType, string name, bool pure, Delegate function)
            : this(returnType, name, pure, function, Array.Empty<LightScriptArgument>())
        {
            
        }

        public LightScriptFunction(LightScriptType returnType, string name, bool pure, Delegate function, params LightScriptType[] arguments)
            : this(returnType, name, pure, function, arguments.Select(argType => new LightScriptArgument(argType)))
        {

        }

        private static bool CheckTypeCompatibility(Type sysType, LightScriptType expectedLSType) => LightScriptTypeSystem.MapFromSystemType(sysType) == expectedLSType;
    }
}