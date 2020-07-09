using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class StringToken : TypeToken
    {
        public StringToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
