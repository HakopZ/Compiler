using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class ClassInformation
    {
        public IdentifierToken ID { get; }

        Dictionary<IdentifierToken, MemberInformation> Members;
        bool AddMember(IdentifierToken id, MemberInformation member)
        {
            if (Members.ContainsKey(id)) return false;
            Members.Add(id, member);
            return true;
        }
        bool TryGetMember(IdentifierToken id, out MemberInformation member)
            => Members.TryGetValue(id, out member);
    }
}
