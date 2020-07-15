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
        public List<ProductionGroup> Rules { get; set; }
        List<Func<TokenCollection, bool>> ProductionsGroupsList { get; set; }

        List<Func<TokenCollection, bool>> ProductionsFound { get; set; }
        public ParseTree()
        {
            Clear();
        }

        
        public void Clear()
        {
            Root = null;
            Count = 0;
            ProductionsGroupsList = new List<Func<TokenCollection, bool>>();
            ProductionsFound = new List<Func<TokenCollection, bool>>();
            Rules = new List<ProductionGroup>();
        }
        public void AddEquation(TokenCollection Equation)
        {
            if (Root is null)
            {
                Root = new NonTerminal(Equation, null);
            }
            var TokenByLine = Equation.GroupBy(x => x.Lexeme == ";").ToList();   
            
            
            while (AddExpression(Equation))
            {

            }
        }
        private bool AddExpression(TokenCollection tokens)
        {
            foreach (var rule in Rules)
            {
                if(rule.TryParse(tokens, out ITerminal node))
                {

                }
            }
            return false;
        }
    }
}
