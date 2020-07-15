using System;
using System.Collections.Generic;
using System.Linq;
using Excersize;
using Excersize.Tokens;
using ParseTreeProject;

namespace ParseTreeProjec
{

    public class ParseTree
    {
        NonTerminal Root;
        public int Count { get; set; }
        public List<Rule> Rules { get; set; }

        List<Func<TokenCollection, (bool Found, int AmountOfTokensUsed)>> ProductionsFound { get; set; }
        public ParseTree()
        {
            Clear();
        }

        
        public void Clear()
        {
            Root = null;
            Count = 0;
            ProductionsFound = new List<Func<TokenCollection, (bool Found, int AmountOfTokensUsed)>>();
            Rules = new List<Rule>();
        }
        public void AddEquation(TokenCollection Equation)
        {
            if (Root is null)
            {
                Root = new NonTerminal(Equation, null);
            }
                            
            while (AddExpression(Equation, out int UsedTokens))
            {
                Equation = (TokenCollection)Equation.Skip(UsedTokens);
            }
        }
        private bool AddExpression(TokenCollection tokens, out int AmountOfTokensUsed)
        {
            foreach (var rule in Rules)
            {
                foreach (var production in rule.ProductionList)
                {
                    if (production(tokens).Found)
                    { 
                        ProductionsFound.Add(production);
                        AmountOfTokensUsed = production(tokens).AmountOfTokensUsed;
                        return true;
                    }
                }
            }
            AmountOfTokensUsed = default;
            return false;
        }
    }
}
