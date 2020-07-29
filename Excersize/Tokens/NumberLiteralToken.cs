using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class NumberLiteralToken : ConstantToken
    {
        public NumberLiteralToken(string lexeme) : base(lexeme)
        {
        }
    }
}
