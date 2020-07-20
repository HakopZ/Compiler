using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class CloseBraceToken : GrammarToken
    {
        public CloseBraceToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
