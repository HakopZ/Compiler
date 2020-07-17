using Excersize;
using ParseTreeProjec;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class NonTerminal : IProductionNode
    {
        public TokenCollection Equation { get; set; }
        public string Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }

        public List<IProductionNode> Children
        {
            get
            {
                return Children;
            }
            set
            {
                Children = value;
            }
        }

        
        public NonTerminal(string id)
        {
            
            Value = id;
            Equation = null;
            Children = new List<IProductionNode>();
        }
    }
}
