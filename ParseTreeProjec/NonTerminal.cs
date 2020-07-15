using Excersize;
using ParseTreeProjec;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class NonTerminal : ITerminal
    {
        public List<Token> Equation { get; set; }
        public List<ITerminal> Children { get; set; } 
        public NonTerminal(List<Token> Eq, List<ITerminal> kids)
        {
            Equation = Eq;
            Children = kids;
        }
    }
}
