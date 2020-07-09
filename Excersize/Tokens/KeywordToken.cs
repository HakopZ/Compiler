using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class KeywordToken : Token
    {
        public KeywordToken(string lexeme)
            : base(lexeme)
        {
            
        }

    }
}
