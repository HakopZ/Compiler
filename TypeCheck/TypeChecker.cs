using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TypeCheck
{

    public struct Scope
    {
        private readonly Dictionary<IdentifierToken, TypeToken> scopeMap;
        public Scope(int capacity = 0)
        {
            scopeMap = new Dictionary<IdentifierToken, TypeToken>(capacity);
        }
        public bool Add(IdentifierToken id, TypeToken type)
        {
            if (scopeMap.ContainsKey(id)) return false;

            scopeMap[id] = type;
            return true;
        }

        public bool TryGetType(IdentifierToken id, out TypeToken type)
            => scopeMap.TryGetValue(id, out type);
    }
    public struct ScopeStack
    {
        private readonly Stack<Scope> scopes;
        public ScopeStack(int capacity = 0)
        {
            scopes = new Stack<Scope>(capacity);
        }
        public void Push(Scope scope)
            => scopes.Push(scope);
        public void Pop()
            => scopes.Pop();
        public bool Add(IdentifierToken ID, TypeToken Type)
            => scopes.Peek().Add(ID, Type);
        public bool TryGetType(IdentifierToken ID, out TypeToken Type)
        {
            Type = default;
            foreach (var scope in scopes)
            {
                if (scope.TryGetType(ID, out Type)) return true;
            }
            return false;
        }
    }

    public class TypeChecker
    {
        public ScopeStack scopeStack;
        private IdentifierToken CurrentID;
        private TypeToken CurrentType;
        private bool FoundType;
        public TypeChecker()
        {
            scopeStack = new ScopeStack(0);
            FoundType = false;
        }
        public void TypeCheck(ParseTreeNode Root)
        {
            
            Find(Root);
        }

        public void Find(ParseTreeNode node)
        {

            if (node.Value is OpenBraceToken)
            {
                scopeStack.Push(new Scope(0));
            }
            else if (node.Value is CloseBraceToken)
            {
                scopeStack.Pop();
            }
            
            else if (node.Value is TypeToken)
            {
                FoundType = true;
                CurrentType = (TypeToken)node.Value;
                if(GetID(node, out CurrentID))
                {

                    scopeStack.Add(CurrentID, CurrentType);
                    CurrentType = default;
                    CurrentID = default;
                }
                else
                {
                    throw new Exception("Didn't find ID");
                }
            }
            foreach (var Kids in node.Children)
            {
                Find(Kids);
            }


            return;
        }
        public bool GetID(ParseTreeNode node, out IdentifierToken ID)
        {
            ID = default;
            if(node.Value is IdentifierToken)
            {
                ID = (IdentifierToken)node.Value;
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
