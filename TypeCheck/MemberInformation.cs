using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public abstract class MemberInformation
    {
        public IdentifierToken ID { get; set; }
        public TypeToken Type { get; set; }
        public bool isStatic { get; set; }
    }
}
