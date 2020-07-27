using System;
using System.Collections.Generic;
using System.Text;

namespace Excersize
{
    public class IdentifierToken : TypeToken
    {
        public IdentifierToken(string lexeme) 
            : base(lexeme)
        {
        }

        public override bool Equals(object obj)
        {
            return (obj is Token token) &&
                   Lexeme == token.Lexeme;
                   
        }

        public override int GetHashCode()
        {
            return Lexeme.GetHashCode();
        }

       
    }
}
