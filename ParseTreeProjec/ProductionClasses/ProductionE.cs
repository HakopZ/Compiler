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
                new Terminal("+"),
                new NonTerminal("E"),
            },
            new Production("E - E")
            {
                new NonTerminal("E"),
                new Terminal("-"),
                new NonTerminal("E")
            }

        };


        public ProductionE(string Ex)
            : base(Ex)
        {
            Productions = new List<Production>();

        }

        public bool TryMatchProduction(TokenCollection tokens, out IProductionNode node)
        {
            foreach (var production in Productions)
            {
                (bool HasLeft, bool HasRight, IProductionNode TNode) = GetTerminal(production);
                if (tokens.TryGetIndexOf(TNode.Value, out int Pos))
                {
                    node = new ExactMatch(tokens[Pos].Lexeme);
                    
                    if (HasLeft)
                    {
                        var LeftSide = new TokenCollection(tokens.Take(Pos));
                         TryMatchProduction(LeftSide, out IProductionNode LeftNode);
                    }
                    if (HasRight)
                    {
                        var RightSide = new TokenCollection(tokens.Skip(Pos + 1));
                        TryMatchProduction(RightSide, out IProductionNode RightNode);
                    }
                }
            }
            node = default;
            return false;
        }
        private (bool HasLeft, bool HasRight, IProductionNode) GetTerminal(Production production)
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
