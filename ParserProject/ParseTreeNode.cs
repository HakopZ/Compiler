using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserProject
{
    public class ParseTreeNode
    {
        public string Value { get; set; }
        public List<ParseTreeNode> Children;
        public List<Token> Expression;
        public bool IsTerminal { get; set; }

        public ParseTreeNode(string val, bool isT)
        {
            Expression = new List<Token>();
            Children = new List<ParseTreeNode>();
            Value = val;
            IsTerminal = isT;
        }
        public void Add(ParseTreeNode node)
        {
            Children.Add(node);
        }
        public void AddExpression(Token node)
        {
            Expression.Add(node);
        }
    }
}
