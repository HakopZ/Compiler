using Excersize;
using ParseTreeProjec;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class NonTerminal : ITerminal
    {
        public TokenCollection Equation { get; set; }
        public List<ITerminal> Children { get; set; } 
        public NonTerminal(TokenCollection Eq, List<ITerminal> kids)
        {
            Equation = Eq;
            Children = kids;
        }
    }
}
