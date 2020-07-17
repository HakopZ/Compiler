using System;
using System.Collections.Generic;
using System.Linq;
using Excersize;
using Excersize.Tokens;
using ParseTreeProject;

namespace ParseTreeProjec
{
    public class Node : IParseTreeNode
    {
        public string Value { get; set; }

        public List<Node> Children;
        public bool IsTerminal { get; set; }
        public Node(string Val)
        {
            Children = new List<Node>();
            Value = Val;
        }
    }

    public class ParseTree
    {
        public int Count { get; set; }
        
        
        public ParseTree()
        {
            Clear();
            
        }

       
        public void Clear()
        {
            Count = 0;
         }
        
    }
}
