using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Excersize
{
    public abstract class TypeToken : Token
    {
        public TypeToken(string lexeme) 
            : base(lexeme)
        {}
    }
}
