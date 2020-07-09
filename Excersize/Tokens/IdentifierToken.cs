using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public class IdentifierToken : Token
    {
        public IdentifierToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
