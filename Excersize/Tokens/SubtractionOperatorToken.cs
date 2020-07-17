using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class SubtractionOperatorToken : OperatorToken
    {
        public SubtractionOperatorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
