using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class Token
    {
        public string Lexeme { get; }
        
        public Token(string lexeme)
        {
            Lexeme = lexeme;
        }
    }
}
