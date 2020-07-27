using Excersize;
using Excersize.Tokens;
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
        public Dictionary<IdentifierToken, MemberInformation> Members = new Dictionary<IdentifierToken, MemberInformation>();

        List<(Type, string)> types = new List<(Type, string)>()
        {
            {(typeof(IntToken), "Int")},
            {(typeof(StringToken), "String") },
            {(typeof(CharKeyWordToken), "Char") },
            {(typeof(BoolToken), "Bool") },
        };
        void AddPrints()
        {
            foreach (var t in types)
            {
                var Print = new MethodInformation() { ID = new IdentifierToken("Print"), isPublic = true, isStatic = true, Type = new VoidKeyWord("void") };
                Print.TryAddParameter(new Parameter() { TypeOf = t.Item1, ID = new IdentifierToken("value") });
                Members.Add(new IdentifierToken($"Print{ t.Item2 }"), Print);
            }
            
        }

        public ClassInformation()
        {

            AddPrints();

            Members.Add(new IdentifierToken("Read"), new MethodInformation() { ID = new IdentifierToken("Read"), isPublic = true, isStatic = true, Type = new StringToken("string") });

            
        }
        public bool AddMember(IdentifierToken id, MemberInformation member)
        {
            if (Members.ContainsKey(id)) return false;
            Members.Add(id, member);
            return true;
        }
        public bool Contains(IdentifierToken ID)
            => Members.ContainsKey(ID);
        public bool TryGetMember(IdentifierToken id, out MemberInformation member)
            => Members.TryGetValue(id, out member);
    }
}
