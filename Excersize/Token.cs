using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public abstract class Token
    {
        public string Lexeme { get; }
        public bool LastTokenInLine { get; set; }
        
        public Token(string lexeme)
        {
            LastTokenInLine = false;
            Lexeme = lexeme;
        }
        
    }
}
