using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class Scope
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
}
