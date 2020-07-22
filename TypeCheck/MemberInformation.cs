using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public abstract class MemberInformation
    {
        string Name { get; set; }
        TypeToken Type { get; set; }
        bool isStatic { get; set; }
    }
}
