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
        public Dictionary<IdentifierToken, MemberInformation> Members = new Dictionary<IdentifierToken, MemberInformation>();

        List<(TypeToken, string)> types = new List<(TypeToken, string)>()
        {
            {(new IntToken("int"), "Int")},
            {(new StringToken("string"), "String") },
            {(new CharKeyWordToken("char"), "Char") },
            {(new BoolToken("bool"), "Bool") },
        };
        void AddPrints()
        {
            foreach (var t in types)
            {
                var Print = new MethodInformation() { ID = new IdentifierToken("Print"),  isStatic = true, Type = new VoidKeyWord("void") };
                Print.TryAddParameter(new Parameter() { TypeOf = t.Item1, ID = new IdentifierToken("value") });
                Members.Add(new IdentifierToken($"Print{ t.Item2 }"), Print);
            }
            
        }

        public ClassInformation()
        {

            AddPrints();

            Members.Add(new IdentifierToken("Read"), new MethodInformation() { ID = new IdentifierToken("Read"),  isStatic = true, Type = new StringToken("string") });

            var ParseMethod = new MethodInformation() { ID = new IdentifierToken("Parse"), isStatic = true, Type = new IntToken("int") };
            ParseMethod.TryAddParameter(new Parameter() { ID = new IdentifierToken("value"), TypeOf = new StringToken("string") });
            Members.Add(ParseMethod.ID, ParseMethod);
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
