using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Excersize;
namespace ParseTreeProject
{
    public class Productions
    {
        public List<Func<Token, bool>> Expression = new List<Func<Token, bool>>();
        
    }
}
