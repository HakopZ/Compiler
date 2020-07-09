using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class NumberLiteralToken : Token
    {
        public NumberLiteralToken(string lexeme) : base(lexeme)
        {
        }
    }
}
