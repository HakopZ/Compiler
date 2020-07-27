using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public class ConstructorToken : TypeToken
    {
        public ConstructorToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
