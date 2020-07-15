using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
using ParseTreeProjec;
using ParseTreeProject;
using System.Text.RegularExpressions;

namespace Testing
{
    public class ParseTreeTest
    {
        
        [Fact]
        public void ParseTreeCheck()
        {
            string Check = "3 + 4";
            RegexTokenizer tokenizer = new RegexTokenizer();
            ReadOnlyMemory<char> temp = Check.AsMemory();//File.ReadAllText("if(x <= 3)");
            TokenCollection tokens = tokenizer.Tokenize(temp);           

            ParseTree tree = new ParseTree();
           Parser parse = new Parser(tokens);   
            
            //rule.AddProduction();
        }

    }
}
