using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class NotToken : GrammarToken
    {
        public NotToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
