using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeCheck
{
    public class MethodInformation : MemberInformation
    {
        Queue<Parameter> parameters;
        public bool IsEntryPoint { get; set; }
        public  List<Parameter> AllParameters { get; set; }
        public MethodInformation()
        {
            AllParameters = new List<Parameter>();
            parameters = new Queue<Parameter>();
        }
        
        public bool TryAddParameter(Parameter param)
        {
            if (parameters.Contains(param)) return false;
            parameters.Enqueue(param);
            AllParameters = parameters.ToList();
            return true;
        }
        public void PopIt()
        {
            if (parameters.Count == 0) return;
            parameters.Dequeue();
        }
       
        public int ParameterCount
            => AllParameters.Count;
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
