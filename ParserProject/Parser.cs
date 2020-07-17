using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserProject
{
    public class Parser
    {
        ProductionGroup E = new ProductionGroup("E")
        {
            new Production("E + E")
            {
                new ProductionNode("E", false),
                new ProductionNode("+", true),
                new ProductionNode("E", false),
            },
            new Production("E - E")
            {
                new ProductionNode("E", false),
                new ProductionNode("-", true),
                new ProductionNode("E", false)
            },
            new Production("EPrime")
            {
                new ProductionNode("EPrime", false)
            }
        };
        public List<ProductionGroup> Groups;
        public TokenCollection Tokens;
        public ProductionGroup Current;
        public Parser(TokenCollection tokens)
        {
            Groups = new List<ProductionGroup>();



            Groups.Add(E);
            Current = Groups[0];
            Tokens = new TokenCollection(Filter(tokens));
        }
        public IEnumerable<Token> Filter(TokenCollection tokens)
        {
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken) && x.GetType() != typeof(CommentToken));
        }


    }
}
