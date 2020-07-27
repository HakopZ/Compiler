using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class EqualOperatorToken : ComparerToken
    {
        public EqualOperatorToken(string lexeme) : base(lexeme)
        {
        }
    }
}
