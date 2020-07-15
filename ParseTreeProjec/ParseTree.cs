using System;
using System.Collections.Generic;
using System.Linq;
using Excersize;
using Excersize.Tokens;
using ParseTreeProject;

namespace ParseTreeProjec
{

    public class ParseTree
    {
        IProductionNode Root;
        public int Count { get; set; }
        
        
        public ParseTree()
        {
            Clear();
            
        }

        
        public void Clear()
        {
            Root = null;
            Count = 0;
         }
        
    }
}
