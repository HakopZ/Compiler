using System;
using System.Collections.Generic;
using Excersize;
using ParseTreeProject;

namespace ParseTreeProjec
{
    
    public class ParseTree
    {
        NonTerminal Root;
        public int Count { get; set; }
        List<Rule> Rules { get; set; }
        public ParseTree()
        {
            Clear();
        }
        public void Clear()
        {
            Root = null;
            Count = 0;
            Rules = new List<Rule>();
        }
        public void AddEquation(List<Token> Equation)
        {
            if(Root is null)
            {
                Root = new NonTerminal(Equation, null);
            }
            foreach (var token in Equation)
            {
                if (!AddToken(token))
                {

                }    

            }
        }
        public bool AddToken(Token token)
        {
            foreach(var rule in Rules)
            {
                foreach (var production in rule.ProductionList)
                {
                    if (production(token))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
