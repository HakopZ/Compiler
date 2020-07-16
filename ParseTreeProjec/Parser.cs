using Excersize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excersize.Tokens;
using System.Threading;

namespace ParseTreeProject
{
    public class Parser
    {
        public TokenCollection Tokens { get; set; }

        public Parser(TokenCollection tokens)
        {
            Tokens = new TokenCollection(Filter(tokens));
        }
        public IEnumerable<Token> Filter(TokenCollection tokens)
        {
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken) && x.GetType() != typeof(CommentToken));

        }
        /*
        public Dictionary<string, ProductionGroup> Map = new Dictionary<string, ProductionGroup>()
        {
            { "E", ProductionGroupDefines.Groups[0] },
            { "EP", ProductionGroupDefines.Groups[1] },
        };

        public bool TryParseProduction(TokenCollection tokens, out IProductionNode node)
        {
            foreach (var item in ProductionGroupDefines.Groups)
            {
                foreach (var p in item.productions)
                {

                }
            }
            node = default;
            return false;
        }
        */


    }
}
