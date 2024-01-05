using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class ElifKeywordToken : ConditionCallToken
    {
        public ElifKeywordToken(string lexeme) : base(lexeme)
        {
        }
    }
}
