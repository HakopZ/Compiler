using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class NotEqualOperatorToken : ComparerToken
    {
        public NotEqualOperatorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
