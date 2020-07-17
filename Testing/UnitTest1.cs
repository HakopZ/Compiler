using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
using System.Text.RegularExpressions;
using System.Linq;
using ParserProject;

namespace Testing
{
    public class ParseTreeTest
    {
        
        
        [Fact]
        public void ParseTreeCheck()
        {
            PlusOperatorToken plus = new PlusOperatorToken("+");
            NumberLiteralToken Three = new NumberLiteralToken("3");
            NumberLiteralToken Four = new NumberLiteralToken("4");

            TokenCollection tokens = new TokenCollection();
            Parser parser = new Parser();
            tokens.Add(Three);
            tokens.Add(plus);
            tokens.Add(Four);

            parser.TryParse(tokens, out ParseTreeNode Tree);
            
            //rule.AddProduction();
        }

    }
}
