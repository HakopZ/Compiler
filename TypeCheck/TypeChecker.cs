using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;

namespace TypeCheck
{
    public class TypeChecker
    {
        public Dictionary<IdentifierToken, TypeToken> Map;
        public TypeChecker()
        {
            Map = new Dictionary<IdentifierToken, TypeToken>();
        }



        public bool TypeCheck(ParseTreeNode Root)
        {
            //if (!Checker(Root)) return false;
            return true;
        }
        public Token Checker(ParseTreeNode node)
        {
            if(node.Value is IdentifierToken)
            {
                IdentifierToken temp = new IdentifierToken(node.Value.Lexeme);
                
                if (Map.ContainsKey(temp)) return default;
                return temp;
            }
            if (node.Children is null)
            {
                return false;
            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                if(node.Children[i].Value is VariableKeyWordToken)
                {
                    Checker(node.Children[i]);
                }
            }
            return false;
        }
    }
}
