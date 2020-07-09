using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class OpenParenthesisToken : GrammarToken
    {
        public OpenParenthesisToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
