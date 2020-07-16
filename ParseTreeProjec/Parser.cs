using Excersize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excersize.Tokens;
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
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken));
        
        }
        public Dictionary<string, ProductionGroup> Map = new Dictionary<string, ProductionGroup>()
        {
            { "E",ProductionGroupDefines.Groups[0]},
            { "EP", ProductionGroupDefines.Groups[1]},
        };        
            
        public bool TryParseProduction(TokenCollection tokens, out IParseTreeNode node)
        {
            
            
            node = default;
            return false;
        }
    }
}
