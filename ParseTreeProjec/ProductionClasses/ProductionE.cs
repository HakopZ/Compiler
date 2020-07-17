using Excersize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseTreeProject.ProductionClasses
{
    public class ProductionE : ProductionGroup
    {
        public List<Production> Productions = new List<Production>()
        {
            new Production("E + E")
            {
                new NonTerminal("E"),
                new ExactMatch("+"),
                new NonTerminal("E"),
            },
            new Production("E - E")
            {
                new NonTerminal("E"),
                new ExactMatch("-"),
                new NonTerminal("E")
            },
            new Production("EPrime")
            {
                new NonTerminal("EPrime")
            }
        };


        public ProductionE(string Ex)
            : base(Ex)
        {
            Productions = new List<Production>();

        }

        
        public  bool TryExactOperatorCheck(TokenCollection tokens, Production production, out IProductionNode node, out TokenCollection Left, out TokenCollection Right)
        {
            (bool HasLeft, bool HasRight, IProductionNode TNode) = GetTerminal(production);
            if (tokens.TryGetIndexOf(TNode.Value, out int pos))
            {

                node = new ExactMatch(tokens[pos].Lexeme);
                Left = null;
                Right = null;
                if (HasLeft)
                {
                    Left = new TokenCollection(tokens.Take(pos));
                }
                if (HasRight)
                {
                    Right = new TokenCollection(tokens.Skip(pos + 1));
                }
                return true;
            }
            node = default;
            Left = Right = default;
            return false;
        }
        private (bool HasLeft, bool HasRight, IProductionNode Node) GetTerminal(Production production)
        {
            for (int i = 0; i < production.Nodes.Count; i++)
            {
                if (production.Nodes[i].GetType() == typeof(ExactMatch))
                {
                    if (i == 0) return (false, true, production.Nodes[i]);
                    else if (i == production.Nodes.Count - 1) return (true, false, production.Nodes[i]);
                    else return (true, true, production.Nodes[i]);
                }
            }
            return (default, default, default);
        }

    }
}
