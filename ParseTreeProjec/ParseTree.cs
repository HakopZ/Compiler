using System;
using System.Collections.Generic;
using Excersize;
using ParseTreeProject;

namespace ParseTreeProjec
{
    public class Node
    {
        public Rule Value { get; set; }
        public List<Token> Children = new List<Token>();
        public Node(Rule value)
        {
            Value = value;
        }
    }
    public class ParseTree
    {
        Node Root;
        public int Count { get; set; }

        public ParseTree()
        {
            Clear();
        }
        public void Clear()
        {
            Root = null;
        }
        public void Add(Rule Value)
        {
            if(Root is null)
            {
                Root = new Node(Value);
            }
            else
            {

            }
            Count++;
        }
        
    }
}
