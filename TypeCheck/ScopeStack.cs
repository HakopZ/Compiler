using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class ScopeStack
    {
        private readonly Stack<Scope> scopes;
        public ScopeStack()
        {
            scopes = new Stack<Scope>();
        }
        public void AddScope(Scope scope)
            => scopes.Push(scope);
        public void LeaveScope()
            => scopes.Pop();
        public bool Add(IdentifierToken ID, TypeToken Type)
            => scopes.Peek().Add(ID, Type);

        public bool Contains(IdentifierToken ID)
        {
            foreach(var scope in scopes)
            {
                if (scope.Contains(ID)) return true;
            }
            return false;
        }
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
}
