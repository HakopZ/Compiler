using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace TypeCheck
{
    public class ClassInformation 
    {
        public IdentifierToken ID { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
        Dictionary<IdentifierToken, MemberInformation> Members = new Dictionary<IdentifierToken, MemberInformation>();
       
        public bool AddMember(IdentifierToken id, MemberInformation member)
        {
            if (Members.ContainsKey(id)) return false;
            Members.Add(id, member);
            return true;
        }
        public bool TryGetMember(IdentifierToken id, out MemberInformation member)
            => Members.TryGetValue(id, out member);
    }
}
