using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class GrammarToken : Token
    {
        public GrammarToken(string lexeme) : base(lexeme)
        {
        }
    }
}
