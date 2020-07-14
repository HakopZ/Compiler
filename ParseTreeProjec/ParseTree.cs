using System;
using System.Collections.Generic;
using Excersize;
using ParseTreeProject;

namespace ParseTreeProjec
{
    public class Node
    {
        public Token Value { get; set; }
        public List<Node> Children = new List<Node>();
        public Rule TokenRule { get; set; }
        public Node(Token value)
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
        public void Add(Token Value)
        {
            
        }
        private void Add(Node current, Token Value)
        {
            if(current.Children == null)
            {
                current.Children.Add(new Node(Value));
                return;
            }
            for (int i = 0; i < current.Children.Count; i++)
            {
                Add(current.Children[i], Value);
            }
        }
        
    }
}
