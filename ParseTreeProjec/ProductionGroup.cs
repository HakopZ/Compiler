using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class ProductionGroup
    {
        Func<TokenCollection, bool> Group { get; set; }
        public ProductionGroup(Func<TokenCollection, bool> func)
        {
            Group = func;
        }

        public bool TryParse(TokenCollection tokens, out ITerminal node)
        {
            if(Group(tokens))
            {
                node = default;
                return true;
            }
            node = default;
            return false;
        }
    }
}
