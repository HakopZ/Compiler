using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TypeCheck
{

    public class TypeChecker
    {
        public ScopeStack scopeStack;

        
        public TypeChecker()
        {
            scopeStack = new ScopeStack(0);
        }
        public bool TypeCheck(IdentifierToken ID, ParseTreeNode Root)
        {
            scopeStack.TryGetType(ID, out TypeToken type);
            
            return false;
        }

        bool FindAssingment(ParseTreeNode node, out Token Constant)
        {
            Constant = default;
            return false;
        }
        public void CallTypeCheck(ParseTreeNode node)
        {
            if (node.Value is OpenBraceToken)
            {
                scopeStack.AddScope(new Scope(0));
            }
            else if (node.Value is CloseBraceToken)
            {
                scopeStack.LeaveScope();
            }
            else if (node.Value is TypeToken)
            {
                TypeToken currentType = (node.Value as TypeToken);
                if(GetID(node, out IdentifierToken id))
                {
                    scopeStack.Add(id, currentType);
                 /*   if(!TypeCheck(id, node))
                    {
                        throw new Exception("Wrong type");
                    }*/
                }
                else
                {
                    throw new Exception("Didn't find ID");
                }

            }
         /*   else if(node.Value is IdentifierToken)
            {
                if(!TypeCheck(node.Value as IdentifierToken, node))
                {
                    throw new Exception("Wrong type");
                }
            }*/
            foreach (var Kids in node.Children)
            {
                CallTypeCheck(Kids);
            }


            return;
        }
        public bool GetID(ParseTreeNode node, out IdentifierToken ID)
        {
            ID = default;
            if(node.Value is IdentifierToken)
            {
                ID = (node.Value as IdentifierToken);
                return true;
            }
            foreach (var Kid in node.Children)
            {
                return GetID(Kid, out ID);
            }
            return false;

        }

    }
}
