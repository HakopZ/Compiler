using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class LessThanOrEqualOperatorToken : ComparerToken
    {
        public LessThanOrEqualOperatorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
