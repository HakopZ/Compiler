using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class CharLiteralToken : Token
    {
        public CharLiteralToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
