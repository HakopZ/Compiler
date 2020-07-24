using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class IfKeyWordToken : ConditionCallToken
    {
        public IfKeyWordToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
