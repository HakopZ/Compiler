using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class ExactMatch : IProductionNode
    {
        public string Value { 
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
        public ExactMatch(string ID)
        {
            Children = new List<IProductionNode>();
            Value = ID;
        }
    }
}
