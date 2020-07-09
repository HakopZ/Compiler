using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class CharLiteral : Token
    {
        public CharLiteral(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
