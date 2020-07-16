using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Terminal : IProductionNode
    {
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
        public Terminal(string value)
        {
            Value = value;
        }
    }
}
