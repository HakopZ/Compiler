using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class MethodInformation : MemberInformation
    {
        Queue<Parameter> parameters;
        public MethodInformation()
        {
            parameters = new Queue<Parameter>();
        }
        
        public bool TryAddParameter(Parameter param)
        {
            if (parameters.Contains(param)) return false;
            parameters.Enqueue(param);
            return true;
        }
        public void PopIt()
        {
            if (parameters.Count == 0) return;
            parameters.Dequeue();
        }
        public bool TryGetParameter(out Parameter param)
        {
            param = default;
            if(parameters.Count == 0)
            {
                return false;
            }
            param = parameters.Dequeue();
            return true;
        }

    }
}
