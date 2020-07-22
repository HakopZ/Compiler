using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public abstract class AccessModifierToken : Token
    {
        public AccessModifierToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
