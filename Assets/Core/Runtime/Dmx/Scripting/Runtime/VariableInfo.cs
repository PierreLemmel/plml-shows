using System;
using UnityEngine;

namespace Plml.Dmx.Scripting.Runtime
{
    public class VariableInfo
    {
        public string name;

        [HideInInspector]
        public bool isReadOnly = false;

        public VariableInfo() { }
        public VariableInfo(string name, bool isReadOnly)
        {
            this.name = name;
            this.isReadOnly = isReadOnly;
        }
    }

    public class VariableInfo<TValue> : VariableInfo
    {        
        public TValue defaultValue;

        public VariableInfo() { }
        public VariableInfo(string name, TValue defaultValue = default, bool isReadOnly = false)
            : base(name, isReadOnly) => this.defaultValue = defaultValue;
    }
}
