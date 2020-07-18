using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
using System.Text.RegularExpressions;
using System.Linq;
using ParserProject;
using System.Diagnostics;

namespace Testing
{
    public class ParseTreeTest
    {

        PlusOperatorToken plus = new PlusOperatorToken("+");
        SubtractionOperatorToken minus = new SubtractionOperatorToken("-");
        MultiplierOperatorToken multiplier = new MultiplierOperatorToken("*");
        DividingOperatorToken divide = new DividingOperatorToken("/");
        OpenParenthesisToken openParenthesisToken = new OpenParenthesisToken("(");
        CloseParenthesisToken closeParenthesisToken = new CloseParenthesisToken(")");
        IdentifierToken a = new IdentifierToken("a");
        IdentifierToken b = new IdentifierToken("b");
        IdentifierToken c = new IdentifierToken("c");
        IdentifierToken d = new IdentifierToken("d");
        IdentifierToken e = new IdentifierToken("e");
        IdentifierToken f = new IdentifierToken("f");
        

        [Fact]
        public void ParseTreeCheck()
        {

            
            //rule.AddProduction();
        }

    }
}
