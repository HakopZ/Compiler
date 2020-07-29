using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TypeCheck;

namespace CodeGen
{
    public static class Generator
    {
        public static AssemblyName assemblyName = new AssemblyName("MyAssembly");
        public static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        public static ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "MyCode.exe");
        public static RegexTokenizer tokenizer = new RegexTokenizer();
        public static Parser parser = new Parser();
        public static TypeValidator typeValidator = new TypeValidator();
        public static void GenerateFromText(string Filename)
        {
            ReadOnlyMemory<char> text = File.ReadAllText(Filename).AsMemory();
            tokenizer.Tokenize(text);


            if (parser.TryParse(tokenizer.tokens, out ParseTreeNode Root))
            {
                Root.Print("", true);

                if (typeValidator.DoProcess(Root))
                {
                    foreach (var ClassNode in Root.Children)
                    {
                        if (GetClassType(ClassNode, out TypeBuilder typeBuilder))
                        {
                            
                        }
                    }
                }
            }
        }
        static bool GetClassType(ParseTreeNode Node, out TypeBuilder typeBuilder)
        {
            typeBuilder = default;

            foreach (var Info in Node.Children)
            {
                if (Info.Value is IdentifierToken)
                {
                    if (typeValidator.symbolTable.TryGetInfo(Info.Value as IdentifierToken, out ClassInformation classInfo))
                    {
                        typeValidator.symbolTable.CurrentClass = classInfo;
                        typeBuilder = moduleBuilder.DefineType(classInfo.ID.Lexeme);
                        return true;
                    }

                }
            }

            return false;
        }
        static bool GetMethodBuilder(ParseTreeNode Node, TypeBuilder typeBuilder, out MethodBuilder methodBuilder)
        {
            methodBuilder = default;
            foreach(var Method in Node.Children)
            {
                if(Method.Value is FunctionKeyWordToken)
                {
                    foreach(var TypeNode in Method.Children)
                    {
                        if(TypeNode.Value is Excersize.TypeToken)
                        {
                            foreach(var Info in TypeNode.Children)
                            {
                                if(Info.Value is IdentifierToken)
                                {
                                    typeValidator.symbolTable.CurrentClass.TryGetMember(Info.Value as IdentifierToken, out MemberInformation member);
                                    var methodInformation = member as MethodInformation;
                                    methodBuilder = typeBuilder.DefineMethod(methodInformation.ID.Lexeme, MethodAttributes.Public); 
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
