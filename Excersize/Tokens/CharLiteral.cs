using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class CharLiteralToken : ConstantToken
    {
        public CharLiteralToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
