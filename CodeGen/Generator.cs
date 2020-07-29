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
    public enum TokenTypeEnum
    {
        Int,
        String,
        Char,
        Bool,
        Identifier
    };
    public enum OperatorsEnum
    { 
        Add,
        Subract,
        Multiplier,
        Divide
    };

    public static class Generator
    {
        public static AssemblyName assemblyName = new AssemblyName("MyAssembly");
        public static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        public static ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "MyCode.exe");
        public static RegexTokenizer tokenizer = new RegexTokenizer();
        public static Parser parser = new Parser();
        public static TypeValidator typeValidator = new TypeValidator();
        static Dictionary<Type, Type> TokenToType = new Dictionary<Type, Type>()
        {
            {typeof(IntToken), typeof(int) },
            {typeof(Excersize.Tokens.StringToken), typeof(string) },
            {typeof(CharKeyWordToken), typeof(char) },
            {typeof(BoolToken), typeof(bool) },
            {typeof(IdentifierToken), typeof(IdentifierToken) }
        };
        static Dictionary<Type, TokenTypeEnum> MapToType = new Dictionary<Type, TokenTypeEnum>()
        {
            {typeof(IntToken), TokenTypeEnum.Int },
            {typeof(Excersize.Tokens.StringToken), TokenTypeEnum.String },
            {typeof(CharKeyWordToken), TokenTypeEnum.Char },
            { typeof(BoolToken), TokenTypeEnum.Bool }
        };
        static Dictionary<Type, OperatorsEnum> MapToOperator = new Dictionary<Type, OperatorsEnum>()
        {
            {typeof(PlusOperatorToken), OperatorsEnum.Add },
            {typeof(SubtractionOperatorToken), OperatorsEnum.Subract },
            {typeof(MultiplierOperatorToken), OperatorsEnum.Multiplier },
            {typeof(DividingOperatorToken), OperatorsEnum.Divide }
        };
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
                        if (GetClassType(ClassNode, out TypeBuilder typeBuilder, out ParseTreeNode IDNode))
                        {
                            foreach (var Member in IDNode.Children)
                            {
                                if (GetMethodBuilder(Member, typeBuilder, out MethodBuilder methodBuilder))
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }
        }
        static bool GetClassType(ParseTreeNode Node, out TypeBuilder typeBuilder, out ParseTreeNode IDNode)
        {
            typeBuilder = default;
            IDNode = default;
            foreach (var Info in Node.Children)
            {
                if (Info.Value is IdentifierToken)
                {
                    if (typeValidator.symbolTable.TryGetInfo(Info.Value as IdentifierToken, out ClassInformation classInfo))
                    {
                        IDNode = Info;
                        typeValidator.symbolTable.CurrentClass = classInfo;
                        typeBuilder = moduleBuilder.DefineType(classInfo.ID.Lexeme);
                        return true;
                    }

                }
            }

            return false;
        }

        static bool EmitFunctionCode(ParseTreeNode node, MethodBuilder methodBuilder)
        {
            var iLGenerator = methodBuilder.GetILGenerator();

            return EmitCode(node, iLGenerator);
        }
        static bool EmitCode(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if(node.Value is ConstantToken)
            {
                var TypeOfConstant = MapToType[typeValidator.TokenTypeMap[node.Value.GetType()]];
                
                switch (TypeOfConstant)
                {
                    case TokenTypeEnum.Int:
                        iLGenerator.Emit(OpCodes.Ldc_I4, int.Parse(node.Value.Lexeme));
                        break;
                    case TokenTypeEnum.String:
                        iLGenerator.Emit(OpCodes.Ldstr, node.Value.Lexeme.Remove(1).Remove(node.Value.Lexeme.Length - 1));
                        break;
                    case TokenTypeEnum.Bool:
                        iLGenerator.Emit(node.Value is TrueKeyWordToken ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                        break;
                    case TokenTypeEnum.Char:
                        //iLGenerator.Emit(OpCodes.lds)
                        break;
                }
                return true;
            }
            bool FailedEmit = false;
            for (int i = 0; i < node.Children.Count; i++)
            {
                if(!EmitCode(node.Children[i], iLGenerator))
                {
                    FailedEmit = true;
                }
            }
            if (node.Value is OperatorToken)
            {
                var TypeOfOperator = MapToOperator[node.Value.GetType()];
                switch (TypeOfOperator)
                {
                    case OperatorsEnum.Add:
                        iLGenerator.Emit(OpCodes.Add);
                        break;
                    case OperatorsEnum.Subract:
                        iLGenerator.Emit(OpCodes.Sub);
                        break;
                    case OperatorsEnum.Multiplier:
                        iLGenerator.Emit(OpCodes.Mul);
                        break;
                    case OperatorsEnum.Divide:
                        iLGenerator.Emit(OpCodes.Div);
                        break;
                }
            }
            else if(node.Value is AssigningOperators)
            {
                iLGenerator.Emit(OpCodes.Stloc);
            }

            return !FailedEmit;
        }
        static bool GetMethodBuilder(ParseTreeNode Node, TypeBuilder typeBuilder, out MethodBuilder methodBuilder)
        {
            methodBuilder = default;

            if (Node.Value is FunctionKeyWordToken)
            {
                foreach (var TypeNode in Node.Children)
                {
                    if (TypeNode.Value is Excersize.TypeToken)
                    {
                        foreach (var Info in TypeNode.Children)
                        {
                            if (Info.Value is IdentifierToken)
                            {
                                typeValidator.symbolTable.CurrentClass.TryGetMember(Info.Value as IdentifierToken, out MemberInformation member);
                                var methodInformation = member as MethodInformation;
                                Type[] parameterTypes = new Type[methodInformation.ParameterCount];
                                for (int i = 0; i < parameterTypes.Length; i++)
                                {
                                    parameterTypes[i] = methodInformation.AllParameters[i].TypeOf.GetType();
                                }
                                methodBuilder = typeBuilder.DefineMethod(methodInformation.ID.Lexeme, methodInformation.isStatic ? MethodAttributes.Static : MethodAttributes.Public, TokenToType[methodInformation.Type.GetType()], parameterTypes);
                                
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
