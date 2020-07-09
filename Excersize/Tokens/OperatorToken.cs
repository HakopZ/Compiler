using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class OperatorToken : Token  
    {
        public OperatorToken(string lexeme)
            :base(lexeme)
        {

        }
    }
}
