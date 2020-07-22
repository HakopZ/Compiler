using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class PublicKeyWordToken : AccessModifierToken
    {
        public PublicKeyWordToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
