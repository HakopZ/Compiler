using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class StaticKeyWordToken : AccessModifierToken
    {
        public StaticKeyWordToken(string lexeme) 
            : base(lexeme)
        {

        }
    }
}
