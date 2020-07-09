using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class TypeToken : Token
    {
        public TypeToken(string lexeme) 
            : base(lexeme)
        {
        }
    }
}
