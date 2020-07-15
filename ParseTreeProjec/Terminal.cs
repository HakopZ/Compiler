using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Terminal : ITerminal
    {
        public int Value { get; set; }
        public Terminal(int value)
        {
            Value = value;
        }
    }
}
