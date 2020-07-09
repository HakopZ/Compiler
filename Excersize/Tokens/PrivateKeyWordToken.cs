using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class PrivateKeyWordToken : KeywordToken
    {
        public PrivateKeyWordToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
