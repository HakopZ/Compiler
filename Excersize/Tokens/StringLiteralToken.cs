using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class StringLiteralToken : ConstantToken
    {
        public StringLiteralToken(string lexeme) : base(lexeme)
        {

        }
    }
}
