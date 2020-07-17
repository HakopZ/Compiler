using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserProject
{
    public class Node
    {
        public string Value { get; set; }
        public List<Node> Children;
        public List<Token> Expression;
        public bool IsTerminal { get; set; }

        public Node(string val, bool isT)
        {
            Expression = new List<Token>();
            Children = new List<Node>();
            Value = val;
            IsTerminal = isT;
        }
    }
}
