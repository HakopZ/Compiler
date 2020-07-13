using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class CommentToken : GrammarToken
    {
        public CommentToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
