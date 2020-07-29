using Excersize;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TypeCheck
{
    public class SymbolTable
    {
        public Dictionary<IdentifierToken, ClassInformation> Map = new Dictionary<IdentifierToken, ClassInformation>();
        ScopeStack scopeStack = new ScopeStack();

        public ClassInformation CurrentClass { get; set; }
        public bool AddInfo(IdentifierToken ID, ClassInformation Info)
        {
            if (Map.ContainsKey(ID)) return false;
            Map[ID] = Info;
            return true;
        }
        public bool TryGetTypeInScope(IdentifierToken ID, out TypeToken type)
            => scopeStack.TryGetType(ID, out type);
        public bool TryGetInfo(IdentifierToken ID, out ClassInformation classInfo)
            => Map.TryGetValue(ID, out classInfo);
        public void EnterScope()
            => scopeStack.AddScope(new Scope());
        public void ExitScope()
            => scopeStack.LeaveScope();
        public bool TryGetMemberInClass(IdentifierToken ID, out MemberInformation info)
        {
            info = default;
            foreach(var m in Map)
            {
                foreach(var member in m.Value.Members)
                {
                    if(member.Key.Lexeme == ID.Lexeme)
                    {
                        info = member.Value;
                        return true;
                    }
                }
            }
            
            return false;
        }
        public bool ContainsAtAll(IdentifierToken ID)
        {
            foreach (var m in Map)
            {
                foreach (var member in m.Value.Members)
                {
                    if (member.Key.Lexeme == ID.Lexeme)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AddInScope(IdentifierToken ID, TypeToken Type)
            => scopeStack.Add(ID, Type);
        

        
    }
}
