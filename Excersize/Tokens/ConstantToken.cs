using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public abstract class ConstantToken : Token
    {
        protected ConstantToken(string lexeme) : base(lexeme)
        {
        }
    }
}
