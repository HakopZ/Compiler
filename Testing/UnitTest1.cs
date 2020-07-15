using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
using ParseTreeProjec;
using ParseTreeProject;

namespace Testing
{
    public class ParseTreeTest
    {
        [Fact]   
        public void CheckForGreaterThan()
        {
            RegexTokenizer tokenizer = new RegexTokenizer();
            ReadOnlyMemory<char> temp = "if(x<=3)".AsMemory();//File.ReadAllText("if(x <= 3)");
            List<Token> tokens = new List<Token>(tokenizer.Tokenize(temp));
            
            Assert.Contains(tokens, (x=> x.GetType() == typeof(LessThanOrEqualOperatorToken)));
        }

        [Fact]
        public void ParseTreeCheck()
        {
            string Check = "3 + 4";
            ParseTree tree = new ParseTree();
            Rule rule = new Rule();
            rule.AddProduction(x => false);
            //rule.AddProduction();
        }
    }
}
