using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class GreatThanOrEqualOperatorToken : ComparerToken
    {
        public GreatThanOrEqualOperatorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
