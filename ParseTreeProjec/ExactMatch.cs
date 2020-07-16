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
        List<IProductionNode> Kids;
        public ExactMatch(string ID)
        {
            Kids = new List<IProductionNode>();
            Value = ID;
        }
    }
}
