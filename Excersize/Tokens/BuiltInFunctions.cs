using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize.Tokens
{
    public abstract class BuiltInFunctions : Token
    {
        protected BuiltInFunctions(string lexeme) : base(lexeme)
        {
        }
    }
}
