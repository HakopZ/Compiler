using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class NotEqualOperatorToken : OperatorToken
    {
        public NotEqualOperatorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
