using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class SymbolTable
    {
        Dictionary<IdentifierToken, ClassInformation> Map = new Dictionary<IdentifierToken, ClassInformation>();
        ScopeStack scopeStack = new ScopeStack();

        public bool AddInfo(IdentifierToken ID, ClassInformation Info)
        {
            if (Map.ContainsKey(ID)) return false;
            Map[ID] = Info;
            return true;
        }
        public bool TryGetInfo(IdentifierToken ID, out ClassInformation classInfo)
            => Map.TryGetValue(ID, out classInfo);
        public void EnterScope()
            => scopeStack.AddScope(new Scope());
        public void ExitScope()
            => scopeStack.LeaveScope();


        
    }
}
