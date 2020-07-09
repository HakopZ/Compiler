using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class SubtractionToken : OperatorToken
    {
        public SubtractionToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
