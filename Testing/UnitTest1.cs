using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
namespace Testing
{
    public class UnitTest1
    {
        [Fact]
        public void CheckForGreaterThan()
        {
            Tokenizer tokenizer = new Tokenizer();
            ReadOnlySpan<char> temp = "if(x<=3)";//File.ReadAllText("if(x <= 3)");
            List<Token> tokens = new List<Token>(tokenizer.Tokenize(temp));
            
            Assert.Contains(tokens, (x=> x.GetType() == typeof(LessThanOrEqualOperatorToken)));
        }
    }
}
