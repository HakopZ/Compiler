using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class MethodInformation : MemberInformation
    {
        Stack<Parameter> parameters;
        public MethodInformation()
        {
            parameters = new Stack<Parameter>();
        }
        
        public bool TryAddParameter(Parameter param)
        {
            if (parameters.Contains(param)) return false;
            parameters.Push(param);
            return true;
        }
        public bool TryGetParameter(out Parameter param)
        {
            param = default;
            if(parameters.Count == 0)
            {
                return false;
            }
            param = parameters.Pop();
            return true;
        }

    }
}
